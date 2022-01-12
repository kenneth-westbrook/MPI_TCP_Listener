namespace VBS.MPI_TCP_Listener.Database
{
    using Microsoft.EntityFrameworkCore;

    public class VBSMPITCPListenerContext : DbContext
    {
        public VBSMPITCPListenerContext()
        {
            
        }
        
        public VBSMPITCPListenerContext(DbContextOptions contextOptions) : base(contextOptions)
        {
        }
        
        public DbSet<HL7MessageDump> Hl7MessageDumps { get; set; }
    }
}