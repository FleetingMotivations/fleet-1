using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FleetEntityFramework.Models;

namespace FleetEntityFramework.DAL
{
    public class FleetContext : DbContext
    {
        public FleetContext() : base("FleetContext") {}

        public IDbSet<Workstation> Workstations { get; set; }
        public IDbSet<Room> Rooms { get; set; }
        public IDbSet<Campus> Campuses { get; set; }
        public IDbSet<Building> Buildings { get; set; }
        public IDbSet<Application> Applications { get; set; }
        public IDbSet<Message> Messages { get; set; }
        public IDbSet<FileMessage> FileMessages { get; set; }
        public IDbSet<AppMessage> AppMessages { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<Workgroup> Workgroups { get; set; }
        public IDbSet<WorkgroupWorkstation> WorkgroupMembers { get; set; }
        public IDbSet<WorkstationLogin> Logins { get; set; }
        public IDbSet<WorkstationMessage> MessageRecords { get; set; }

    }
}
