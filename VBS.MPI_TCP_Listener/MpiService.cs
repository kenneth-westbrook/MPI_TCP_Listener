namespace VBS.MPI_TCP_Listener
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using HL7.Dotnetcore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using VBS.MPI_TCP_Listener.Database;

    public class MpiService : BackgroundService
    {
        private readonly ILogger<MpiService> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly AppSettings settings;
        private TcpListener server;

        public MpiService(ILogger<MpiService> logger, IOptions<AppSettings> settings,
                          IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.settings = settings.Value;
            this.serviceScopeFactory = serviceScopeFactory;
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting service");
            var localAddr = IPAddress.Parse(this.settings.ListeningIPAddress);

            server = new TcpListener(localAddr, this.settings.ListeningPort);
            server.Start();
            logger.LogInformation($"TCP Server started.  Listening on {this.server.LocalEndpoint}");

            while (!cancellationToken.IsCancellationRequested)
            {
                if (await this.WaitForConnections(cancellationToken) == false)
                {
                    break;
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.server?.Stop();
            this.logger.LogInformation("Stopped Service");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private async Task AcknowledgeMessage(CancellationToken cancellationToken, NetworkStream stream, string messageStr)
        {
            var message = new Message(messageStr);
            try
            {
                message.ParseMessage();
            }
            catch(Exception exception)
            {
                logger.LogError(exception, "Error while parsing message for acknowledging process");
            }

            // Commit Ack message needs to go through the same socket.  This message just tells the sender that
            // we received the message.  Does not indicate successfull processing.  The API will handle that.
            try
            {
                var comAckMessage = GenerateAcknowledgeMessage(message).SerializeMessage(false);
                logger.LogDebug(comAckMessage);
                if (!string.IsNullOrEmpty(comAckMessage))
                {
                    var byteComAckMessage = comAckMessage.GetEncodedOutput();
                    await stream.WriteAsync(byteComAckMessage, 0, byteComAckMessage.Length, cancellationToken);
                    logger.LogDebug("Sent CA Ack");
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error sending commit ack message");
            }
        }

        private async Task GetMessages(CancellationToken cancellationToken, NetworkStream stream, VBSMPITCPListenerContext dbContext)
        {
            var receivedByteBuffer = new byte[200];

            var data = string.Empty;
            var data2 = string.Empty;
            int bytesReceived; // Received byte count
            while ((bytesReceived = await stream.ReadAsync(receivedByteBuffer, 0, receivedByteBuffer.Length, cancellationToken)) > 0)
            {
                data += Encoding.UTF8.GetString(receivedByteBuffer, 0, bytesReceived);
                data2 += Encoding.ASCII.GetString(receivedByteBuffer, 0, bytesReceived);
                
                // this character is used to indicate that it is the end of all messages sent,
                // so when we encounter this we want to stop waiting for further input and move on to storing and acknowledging the received messages
                var end = data.IndexOf((char)0x1C);
                if (end <= 0)
                {
                    continue;
                }
                
                await HandleMessages(cancellationToken, data, dbContext, stream);
                data = string.Empty;
            }
        }

        public static string[] ExtractMessages(string messages)
        {
            string pattern = "\v(.*?)\x001C\r";
            MatchCollection matchCollection = Regex.Matches(messages, pattern, RegexOptions.Singleline);
            List<string> stringList = new List<string>();
            foreach (Match match in matchCollection)
                stringList.Add(match.Groups[1].Value);
            return stringList.ToArray();
        }

        private async Task HandleMessages(CancellationToken cancellationToken, string data, VBSMPITCPListenerContext dbContext,
                                          NetworkStream stream)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            //var messages = MessageHelper.ExtractMessages(data);
            var messages = ExtractMessages(data);

            if (messages.Length == 0)
            {
                return;
            }

            //try
            //{
            //    await dbContext.Hl7MessageDumps
            //                   .AddRangeAsync(messages
            //                                  .Where(x => !x.Contains("HLO PING CLIENT"))
            //                                  .Select(x => new HL7MessageDump
            //                                  {
            //                                      Content = x,
            //                                      MessageTypeId = (int)Hl7MessageTypeValue.Demographics,
            //                                      Recieved = DateTime.UtcNow,
            //                                      Processed = DateTime.UtcNow,
            //                                  }),
            //                                  cancellationToken);

            //    await dbContext.SaveChangesAsync(cancellationToken);
            //}
            //catch (Exception e)
            //{
            //    // failed to save to database, don't acknowledge, NACK instead
            //}

            foreach (var messageStr in messages)
            {
                await AcknowledgeMessage(cancellationToken, stream, messageStr);
            }
        }

        private async Task<bool> WaitForConnections(CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<VBSMPITCPListenerContext>();

            try
            {
                cancellationToken.Register(() => server.Stop());
                logger.LogDebug("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                var client = await server.AcceptTcpClientAsync();
                logger.LogDebug("Connected!");

                var stream = client.GetStream();

                await GetMessages(cancellationToken, stream, dbContext);
                
                //// notify API to parse dumped messages
                //try
                //{

                //    if (!string.IsNullOrWhiteSpace(settings.MpiEndPoint))
                //    {
                //        var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                //        var httpClient = clientFactory.CreateClient();
                //        httpClient.Timeout = TimeSpan.FromMinutes(60);
                //        var response = await httpClient.PostAsync(new Uri(settings.MpiEndPoint), new StringContent(""), cancellationToken);
                //        logger.LogDebug("Notified API to process dumped HL7 messages.");
                //    }
                //}
                //catch(Exception ex)
                //{
                //    logger.LogError("Exception: {0}", ex);
                //}

                client.Close();
                logger.LogInformation("Connection closed");
            }
            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Connection closed");
                return false;
            }
            catch (Exception e)
            {
                logger.LogError("Exception: {0}", e);
            }

            return true;
        }

        private Message GenerateAcknowledgeMessage(Message msg)
        {
            var response = new StringBuilder();
            var dateString = MessageHelper.LongDateWithFractionOfSecond(DateTime.Now);
            var delim = msg.Encoding.FieldDelimiter;
           
            response.Append("MSH");
            response.Append(@"^~|\&^");
            response.Append(msg.GetValue("MSH.5"));
            response.Append(delim);
            response.Append(msg.GetValue("MSH.6"));
            response.Append(delim);
            response.Append(msg.GetValue("MSH.3"));
            response.Append(delim);
            response.Append(msg.GetValue("MSH.4"));
            response.Append(delim);
            response.Append(dateString);
            response.Append(delim);
            response.Append(delim);
            response.Append("ACK");
            response.Append(delim);
            response.Append(GetNewMessageControlId());
            response.Append(delim);
            response.Append(msg.GetValue("MSH.11"));
            response.Append(delim);
            response.Append(msg.GetValue("MSH.12"));
            response.Append(msg.Encoding.SegmentDelimiter);

            response.Append("MSA");
            response.Append(delim);
            response.Append(AcknowledgeMessageCodes.Commit);
            response.Append(delim);
            response.Append(msg.GetValue("MSH.10"));
            response.Append(msg.Encoding.SegmentDelimiter);
            try
            {
                var message = new Message(response.ToString());
                message.ParseMessage();
                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while parsing and acknowledging message : {response}");
                return null;
            }
        }

        private string GetNewMessageControlId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
        }
    }
}