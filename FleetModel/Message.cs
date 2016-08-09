namespace FleetModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            WorkStationMessages = new HashSet<WorkStationMessage>();
        }

        public int Id { get; set; }

        [Required]
        public string TargetApplication { get; set; }

        public DateTime TimeSent { get; set; }

        public virtual Messages_AppMessage Messages_AppMessage { get; set; }

        public virtual Messages_FileMessage Messages_FileMessage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkStationMessage> WorkStationMessages { get; set; }
    }
}
