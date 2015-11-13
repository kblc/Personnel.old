namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeStuffingForEmployee : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.stuffing", "employee_id", "dbo.employee");
            DropIndex("dbo.stuffing", new[] { "employee_id" });
            AddColumn("dbo.employee", "stuffing_id", c => c.Long(nullable: true));
            CreateIndex("dbo.employee", "stuffing_id");
            AddForeignKey("dbo.employee", "stuffing_id", "dbo.stuffing", "stuffing_id", cascadeDelete: false);
            DropColumn("dbo.stuffing", "employee_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.stuffing", "employee_id", c => c.Long(nullable: true));
            DropForeignKey("dbo.employee", "stuffing_id", "dbo.stuffing");
            DropIndex("dbo.employee", new[] { "stuffing_id" });
            DropColumn("dbo.employee", "stuffing_id");
            CreateIndex("dbo.stuffing", "employee_id");
            AddForeignKey("dbo.stuffing", "employee_id", "dbo.employee", "employee_id");
        }
    }
}
