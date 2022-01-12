namespace VBS.MPI_TCP_Listener
{
    public class AppSettings
    {
        public string MpiEndPoint { get; set; }
        
        public int ListeningPort { get; set; }
        
        public string ListeningIPAddress { get; set; }

        public string MpiAckTcpAddress { get; set; }

        public int MpiAckTcpPort { get; set; }
    }
}