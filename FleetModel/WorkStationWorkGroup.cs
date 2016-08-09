namespace FleetModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WorkStationWorkGroup
    {
        public int Id { get; set; }

        public DateTime? TimeRemoved { get; set; }

        public bool CanShare { get; set; }

        public int WorkstationId { get; set; }

        public int WorkGroupId { get; set; }

        public int RoomId { get; set; }

        public virtual Room Room { get; set; }

        public virtual WorkGroup WorkGroup { get; set; }

        public virtual Workstation Workstation { get; set; }
    }
}
