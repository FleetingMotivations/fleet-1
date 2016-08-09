using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FleetEntityFramework.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public string ApplicationName { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Workgroup> Workgroups { get; set; }
    }
}
