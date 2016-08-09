using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FleetEntityFramework.Models
{
    public class Workgroup
    {
        [Key]
        public int WorkgroupId { get; set; }

        [Required]
        public DateTime Started { get; set; }
        [Required]
        public DateTime Expires { get; set; }

        public int CommisionedById { get; set; }
        public User CommisionedBy { get; set; }

        public virtual ICollection<WorkgroupWorkstation> Workstations { get; set; }
        public virtual ICollection<Application> AllowedApplications { get; set; }
    }
}
