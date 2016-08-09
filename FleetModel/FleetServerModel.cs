namespace FleetModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FleetServerModel : DbContext
    {
        public FleetServerModel()
            : base("name=FleetServerModel")
        {
        }

        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Campu> Campus { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Messages_AppMessage> Messages_AppMessage { get; set; }
        public virtual DbSet<Messages_FileMessage> Messages_FileMessage { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WorkGroup> WorkGroups { get; set; }
        public virtual DbSet<WorkStationMessage> WorkStationMessages { get; set; }
        public virtual DbSet<Workstation> Workstations { get; set; }
        public virtual DbSet<WorkStationWorkGroup> WorkStationWorkGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Building>()
                .HasMany(e => e.Rooms)
                .WithRequired(e => e.Building)
                .HasForeignKey(e => e.Building_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Campu>()
                .HasMany(e => e.Buildings)
                .WithRequired(e => e.Campu)
                .HasForeignKey(e => e.CampusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                .HasOptional(e => e.Messages_AppMessage)
                .WithRequired(e => e.Message)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Message>()
                .HasOptional(e => e.Messages_FileMessage)
                .WithRequired(e => e.Message)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Message>()
                .HasMany(e => e.WorkStationMessages)
                .WithRequired(e => e.Message)
                .HasForeignKey(e => e.MessageId1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.WorkStationWorkGroups)
                .WithRequired(e => e.Room)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WorkGroups)
                .WithMany(e => e.Users)
                .Map(m => m.ToTable("UserWorkGroup").MapLeftKey("Users_Id").MapRightKey("WorkGroups_Id"));

            modelBuilder.Entity<WorkGroup>()
                .HasMany(e => e.WorkStationWorkGroups)
                .WithRequired(e => e.WorkGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workstation>()
                .HasMany(e => e.WorkStationMessages)
                .WithRequired(e => e.Workstation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workstation>()
                .HasMany(e => e.WorkStationWorkGroups)
                .WithRequired(e => e.Workstation)
                .WillCascadeOnDelete(false);
        }
    }
}
