using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FleetEntityFramework.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set;  }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Index(IsUnique=true)]
        public string RoomIdentifier { get; set; }


        [Required]
        public int BuildingId { get; set; }
        public virtual Building Building { get; set; }

        public virtual ICollection<Workstation> Workstations { get; set; }
    }
}
