namespace FleetEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkstationFriendlyName : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Workstations", new[] { "MacAddress" });
            DropIndex("dbo.Workstations", new[] { "RoomID" });
            AddColumn("dbo.Workstations", "FriendlyName", c => c.String());
            AlterColumn("dbo.Workstations", "MacAddress", c => c.String(maxLength: 450, unicode: false));
            CreateIndex("dbo.Workstations", "MacAddress");
            CreateIndex("dbo.Workstations", "RoomID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workstations", new[] { "RoomID" });
            DropIndex("dbo.Workstations", new[] { "MacAddress" });
            AlterColumn("dbo.Workstations", "MacAddress", c => c.String(nullable: false, maxLength: 450, unicode: false));
            DropColumn("dbo.Workstations", "FriendlyName");
            CreateIndex("dbo.Workstations", "RoomID");
            CreateIndex("dbo.Workstations", "MacAddress", unique: true);
        }
    }
}
