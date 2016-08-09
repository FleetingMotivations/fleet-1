using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetEntityFramework.Models
{
    public class WorkstationMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkStationId { get; set; }
        public Workstation Target { get; set; }

        [Required]
        public int MessageId { get; set; }
        public Message Message { get; set; }

        // The DateTime at which the message was collected by the Target
        public DateTime Received { get; set; } 
        public bool HasBeenSeen { get; set; }
    }
}
