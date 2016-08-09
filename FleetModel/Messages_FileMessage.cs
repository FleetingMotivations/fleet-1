namespace FleetModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Messages_FileMessage
    {
        [Required]
        public string FileType { get; set; }

        [Required]
        public string FileName { get; set; }

        public int FileSizeBytes { get; set; }

        public bool HasBeenScanned { get; set; }

        [Required]
        public string URI { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public virtual Message Message { get; set; }
    }
}
