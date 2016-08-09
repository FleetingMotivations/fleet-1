namespace FleetModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Workstation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workstation()
        {
            WorkStationMessages = new HashSet<WorkStationMessage>();
            WorkStationWorkGroups = new HashSet<WorkStationWorkGroup>();
        }

        public int Id { get; set; }

        [Required]
        public string ComputerName { get; set; }

        [Required]
        public string IPAddress { get; set; }

        [Required]
        public string MACAddress { get; set; }

        public DateTime LastSeen { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkStationMessage> WorkStationMessages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkStationWorkGroup> WorkStationWorkGroups { get; set; }
    }
}
