using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetEntityFramework.Models
{
    public class Workstation
    {
        [Key]
        public int WorkstationId { get; set; }

        public string FriendlyName { get; set; }

        [Required]
        [Column(TypeName ="VARCHAR")]
        [StringLength(100)]
        [Index(IsUnique = true )]
        public string WorkstationIdentifier { get; set; }

        [Index]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Required]
        public string IpAddress { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(450)]
        [Index]
        public string MacAddress { get; set; }

        public int? RoomID { get; set; }
        public virtual Room Room { get; set; }

        public DateTime LastSeen { get; set; }

        public virtual ICollection<WorkgroupWorkstation> Workgroups { get; set; }
        public virtual ICollection<WorkstationLogin> Logins { get; set; }

        public virtual ICollection<WorkstationMessage> ReceivedMessages { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
    }
}
