namespace FleetModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WorkStationMessage
    {
        public int Id { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string MessageId { get; set; }

        public bool HasBeenSeen { get; set; }

        [Required]
        public string Received { get; set; }

        public int MessageId1 { get; set; }

        public int WorkstationId { get; set; }

        public virtual Message Message { get; set; }

        public virtual Workstation Workstation { get; set; }
    }
}
