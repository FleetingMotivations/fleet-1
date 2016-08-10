using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetEntityFramework.Models
{
    public abstract class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int WorkstationId { get; set; }
        public virtual Workstation Sender { get; set; }

        [Required]
        public int ApplicationId { get; set; }
        public virtual Application TargetApplication { get; set; }

        [Required]
        public DateTime Sent { get; set; }

        public virtual ICollection<WorkstationMessage> MessageRecords { get; set; } 
    }

    public class FileMessage : Message
    {
        [Required]
        public string FileName { get; set; }
        
        [Required]
        public string FileType { get; set; }

        // In bytes?
        [Required]
        public int FileSize { get; set; }

        public bool HasBeenScanned { get; set; }

        [Required]
        public string Uri { get; set; }
    }

    public class AppMessage : Message
    {
        [Required]
        public string Message { get; set; }
    }
}
