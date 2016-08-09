using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FleetEntityFramework.Models
{
    public class WorkstationLogin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkstationId { get; set; }
        public virtual Workstation Workstation { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime LoginTime { get; set; }
    }
}
