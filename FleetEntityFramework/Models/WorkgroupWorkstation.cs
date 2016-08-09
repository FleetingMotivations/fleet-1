using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetEntityFramework.Models
{
    public class WorkgroupWorkstation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkstationId { get; set; }
        public virtual Workstation Workstation { get; set; }

        [Required]
        public int WorkgroupId { get; set; }
        public virtual Workgroup Workgroup { get; set; }

        public DateTime? TimeRemoved { get; set; }

        public bool SharingEnabled { get; set; }
    }
}
