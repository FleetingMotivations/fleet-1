namespace FleetEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeRoomIdNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Workstations", "RoomID", "dbo.Rooms");
            DropIndex("dbo.Workstations", new[] { "RoomID" });
            AlterColumn("dbo.Workstations", "RoomID", c => c.Int());
            CreateIndex("dbo.Workstations", "RoomID");
            AddForeignKey("dbo.Workstations", "RoomID", "dbo.Rooms", "RoomId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Workstations", "RoomID", "dbo.Rooms");
            DropIndex("dbo.Workstations", new[] { "RoomID" });
            AlterColumn("dbo.Workstations", "RoomID", c => c.Int(nullable: false));
            CreateIndex("dbo.Workstations", "RoomID");
            AddForeignKey("dbo.Workstations", "RoomID", "dbo.Rooms", "RoomId", cascadeDelete: true);
        }
    }
}
