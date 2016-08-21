namespace FleetEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageReceivedNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkstationMessages", "Received", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkstationMessages", "Received", c => c.DateTime(nullable: false));
        }
    }
}
