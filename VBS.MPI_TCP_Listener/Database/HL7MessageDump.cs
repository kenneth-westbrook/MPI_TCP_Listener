namespace VBS.MPI_TCP_Listener.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class HL7MessageDump
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Content { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime Recieved { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Processed { get; set; }

        [Column("MessageType_Id")]
        public int? MessageTypeId { get; set; }

        public virtual HL7MessageType MessageType { get; set; }
    }
}