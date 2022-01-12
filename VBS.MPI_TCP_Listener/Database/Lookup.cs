namespace VBS.MPI_TCP_Listener.Database
{
    using System.ComponentModel.DataAnnotations;

    public abstract class Lookup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual string LookupName { get; set; }

        [Required]
        public virtual string Description { get; set; }
    }
}