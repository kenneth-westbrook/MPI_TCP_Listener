namespace VBS.MPI_TCP_Listener
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using System;
    using VBS.MPI_TCP_Listener.Database;

    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseSerilog()
                       .UseWindowsService()
                       .ConfigureAppConfiguration((context, config) =>
                       {
                           // configuration stuff
                       })
                       .ConfigureServices((hostContext, services) =>
                       {
                           services.AddHostedService<MpiService>();
                           services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
                           services.AddHttpClient();
                           
                           var connectionString = string.IsNullOrWhiteSpace(hostContext.Configuration.GetConnectionString("VBSMPITCPListenerContext"))
                               ? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=VBS_MPI_Data;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
                               : hostContext.Configuration.GetConnectionString("VBSMPITCPListenerContext");

                           services.AddDbContext<VBSMPITCPListenerContext>(opt => opt.UseSqlServer(connectionString));
                       });
        }

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            //Serilog configuration is all in appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Application is STarting Up");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal("Application Failed to Start Up");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}