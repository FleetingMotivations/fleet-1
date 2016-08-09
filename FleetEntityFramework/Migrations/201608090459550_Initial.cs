namespace FleetEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        ApplicationId = c.Int(nullable: false, identity: true),
                        ApplicationName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        WorkstationId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        Sent = c.DateTime(nullable: false),
                        Message = c.String(),
                        FileName = c.String(),
                        FileType = c.String(),
                        FileSize = c.Int(),
                        HasBeenScanned = c.Boolean(),
                        Uri = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Workstations", t => t.WorkstationId, cascadeDelete: true)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.WorkstationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Workstations",
                c => new
                    {
                        WorkstationId = c.Int(nullable: false, identity: true),
                        WorkstationIdentifier = c.String(nullable: false, maxLength: 100, unicode: false),
                        IpAddress = c.String(nullable: false, maxLength: 100, unicode: false),
                        MacAddress = c.String(nullable: false, maxLength: 450, unicode: false),
                        RoomID = c.Int(nullable: false),
                        LastSeen = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WorkstationId)
                .ForeignKey("dbo.Rooms", t => t.RoomID, cascadeDelete: true)
                .Index(t => t.WorkstationIdentifier, unique: true)
                .Index(t => t.IpAddress)
                .Index(t => t.MacAddress, unique: true)
                .Index(t => t.RoomID);
            
            CreateTable(
                "dbo.WorkstationLogins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkstationId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        LoginTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Workstations", t => t.WorkstationId, cascadeDelete: true)
                .Index(t => t.WorkstationId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Identifer = c.String(nullable: false, maxLength: 100, unicode: false),
                        FirstName = c.String(nullable: false, maxLength: 100, unicode: false),
                        LastName = c.String(nullable: false, maxLength: 100, unicode: false),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Identifer, unique: true)
                .Index(t => new { t.FirstName, t.LastName }, name: "IX_UserFirstLastNameIndex");
            
            CreateTable(
                "dbo.WorkstationMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkStationId = c.Int(nullable: false),
                        MessageId = c.Int(nullable: false),
                        Received = c.DateTime(nullable: false),
                        HasBeenSeen = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .ForeignKey("dbo.Workstations", t => t.WorkStationId, cascadeDelete: false)
                .Index(t => t.WorkStationId)
                .Index(t => t.MessageId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        RoomIdentifier = c.String(nullable: false, maxLength: 100, unicode: false),
                        BuildingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Buildings", t => t.BuildingId, cascadeDelete: true)
                .Index(t => t.RoomIdentifier, unique: true)
                .Index(t => t.BuildingId);
            
            CreateTable(
                "dbo.Buildings",
                c => new
                    {
                        BuildingId = c.Int(nullable: false, identity: true),
                        BuildingIdentifier = c.String(nullable: false, maxLength: 100, unicode: false),
                        CampusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BuildingId)
                .ForeignKey("dbo.Campus", t => t.CampusId, cascadeDelete: true)
                .Index(t => t.BuildingIdentifier, unique: true)
                .Index(t => t.CampusId);
            
            CreateTable(
                "dbo.Campus",
                c => new
                    {
                        CampusId = c.Int(nullable: false, identity: true),
                        CampusIdentifer = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CampusId);
            
            CreateTable(
                "dbo.WorkgroupWorkstations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkstationId = c.Int(nullable: false),
                        WorkgroupId = c.Int(nullable: false),
                        TimeRemoved = c.DateTime(),
                        SharingEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Workgroups", t => t.WorkgroupId, cascadeDelete: true)
                .ForeignKey("dbo.Workstations", t => t.WorkstationId, cascadeDelete: true)
                .Index(t => t.WorkstationId)
                .Index(t => t.WorkgroupId);
            
            CreateTable(
                "dbo.Workgroups",
                c => new
                    {
                        WorkgroupId = c.Int(nullable: false, identity: true),
                        Started = c.DateTime(nullable: false),
                        Expires = c.DateTime(nullable: false),
                        CommisionedById = c.Int(nullable: false),
                        CommisionedBy_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.WorkgroupId)
                .ForeignKey("dbo.Users", t => t.CommisionedBy_UserId)
                .Index(t => t.CommisionedBy_UserId);
            
            CreateTable(
                "dbo.WorkgroupApplications",
                c => new
                    {
                        Workgroup_WorkgroupId = c.Int(nullable: false),
                        Application_ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Workgroup_WorkgroupId, t.Application_ApplicationId })
                .ForeignKey("dbo.Workgroups", t => t.Workgroup_WorkgroupId, cascadeDelete: true)
                .ForeignKey("dbo.Applications", t => t.Application_ApplicationId, cascadeDelete: true)
                .Index(t => t.Workgroup_WorkgroupId)
                .Index(t => t.Application_ApplicationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.WorkgroupWorkstations", "WorkstationId", "dbo.Workstations");
            DropForeignKey("dbo.WorkgroupWorkstations", "WorkgroupId", "dbo.Workgroups");
            DropForeignKey("dbo.Workgroups", "CommisionedBy_UserId", "dbo.Users");
            DropForeignKey("dbo.WorkgroupApplications", "Application_ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.WorkgroupApplications", "Workgroup_WorkgroupId", "dbo.Workgroups");
            DropForeignKey("dbo.Messages", "WorkstationId", "dbo.Workstations");
            DropForeignKey("dbo.Workstations", "RoomID", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "BuildingId", "dbo.Buildings");
            DropForeignKey("dbo.Buildings", "CampusId", "dbo.Campus");
            DropForeignKey("dbo.WorkstationMessages", "WorkStationId", "dbo.Workstations");
            DropForeignKey("dbo.WorkstationMessages", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.WorkstationLogins", "WorkstationId", "dbo.Workstations");
            DropForeignKey("dbo.WorkstationLogins", "UserId", "dbo.Users");
            DropIndex("dbo.WorkgroupApplications", new[] { "Application_ApplicationId" });
            DropIndex("dbo.WorkgroupApplications", new[] { "Workgroup_WorkgroupId" });
            DropIndex("dbo.Workgroups", new[] { "CommisionedBy_UserId" });
            DropIndex("dbo.WorkgroupWorkstations", new[] { "WorkgroupId" });
            DropIndex("dbo.WorkgroupWorkstations", new[] { "WorkstationId" });
            DropIndex("dbo.Buildings", new[] { "CampusId" });
            DropIndex("dbo.Buildings", new[] { "BuildingIdentifier" });
            DropIndex("dbo.Rooms", new[] { "BuildingId" });
            DropIndex("dbo.Rooms", new[] { "RoomIdentifier" });
            DropIndex("dbo.WorkstationMessages", new[] { "MessageId" });
            DropIndex("dbo.WorkstationMessages", new[] { "WorkStationId" });
            DropIndex("dbo.Users", "IX_UserFirstLastNameIndex");
            DropIndex("dbo.Users", new[] { "Identifer" });
            DropIndex("dbo.WorkstationLogins", new[] { "UserId" });
            DropIndex("dbo.WorkstationLogins", new[] { "WorkstationId" });
            DropIndex("dbo.Workstations", new[] { "RoomID" });
            DropIndex("dbo.Workstations", new[] { "MacAddress" });
            DropIndex("dbo.Workstations", new[] { "IpAddress" });
            DropIndex("dbo.Workstations", new[] { "WorkstationIdentifier" });
            DropIndex("dbo.Messages", new[] { "ApplicationId" });
            DropIndex("dbo.Messages", new[] { "WorkstationId" });
            DropTable("dbo.WorkgroupApplications");
            DropTable("dbo.Workgroups");
            DropTable("dbo.WorkgroupWorkstations");
            DropTable("dbo.Campus");
            DropTable("dbo.Buildings");
            DropTable("dbo.Rooms");
            DropTable("dbo.WorkstationMessages");
            DropTable("dbo.Users");
            DropTable("dbo.WorkstationLogins");
            DropTable("dbo.Workstations");
            DropTable("dbo.Messages");
            DropTable("dbo.Applications");
        }
    }
}
