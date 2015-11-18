namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_DeleteAppointTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.stuffing", "appoint_id", "dbo.appoint");
            DropIndex("dbo.stuffing", new[] { "appoint_id" });
            AddColumn("dbo.stuffing", "appoint", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.stuffing", "appoint_id");
            DropTable("dbo.appoint");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.appoint",
                c => new
                    {
                        appoint_id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.appoint_id);
            
            AddColumn("dbo.stuffing", "appoint_id", c => c.Long(nullable: false));
            DropColumn("dbo.stuffing", "appoint");
            CreateIndex("dbo.stuffing", "appoint_id");
            AddForeignKey("dbo.stuffing", "appoint_id", "dbo.appoint", "appoint_id", cascadeDelete: true);
        }
    }
}
