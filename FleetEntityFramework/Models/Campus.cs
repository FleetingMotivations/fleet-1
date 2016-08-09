using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetEntityFramework.Models
{
    public class Campus
    {
        [Key]
        public int CampusId { get; set; }

        [Required]
        public string CampusIdentifer { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
    }
}
