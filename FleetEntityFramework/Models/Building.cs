using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FleetEntityFramework.Models
{
    public class Building
    {
        [Key]
        public int BuildingId { get; set; }

        [Index(IsUnique = true)]
        [Required]
        public string BuildingIdentifier { get; set; }

        [Required]
        public int CampusId { get; set; }
        public virtual Campus Campus { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
