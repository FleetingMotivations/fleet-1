using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetEntityFramework.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [Index(IsUnique = true)]
        public string Identifer { get; set; }

        [Required]
        [Index("IX_UserFirstLastNameIndex", 1)]
        public string FirstName { get; set; }
        [Index("IX_UserFirstLastNameIndex", 2)]
        public string LastName { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public virtual ICollection<WorkstationLogin> Logins { get; set; }
    }

    [Flags]
    public enum UserRole
    {
        Facilitator = 0x1,
        Regular = 0x2
    }
}
