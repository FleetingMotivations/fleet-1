namespace FleetModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Room()
        {
            WorkStationWorkGroups = new HashSet<WorkStationWorkGroup>();
        }

        public int Id { get; set; }

        [Required]
        public string Identifier { get; set; }

        public int Building_Id { get; set; }

        public virtual Building Building { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkStationWorkGroup> WorkStationWorkGroups { get; set; }
    }
}
