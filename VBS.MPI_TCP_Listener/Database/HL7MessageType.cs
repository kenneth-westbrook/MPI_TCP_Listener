namespace VBS.MPI_TCP_Listener.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class HL7MessageType : Lookup
    {
        [Column("HL7MessageType")]
        public override string LookupName { get => base.LookupName; set => base.LookupName = value; }

        public override string Description
        { get => base.Description; set => base.Description = value; }

        [NotMapped]
        public Hl7MessageTypeValue Value
        {
            get
            {
                return (Hl7MessageTypeValue)this.Id;
            }

            set
            {
                this.Id = (int)value;
            }
        }

        public virtual ICollection<HL7MessageDump> Hl7MessageDump { get; set; }
    }
}