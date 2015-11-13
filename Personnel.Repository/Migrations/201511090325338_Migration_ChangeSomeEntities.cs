namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeSomeEntities : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.employee_login", new[] { "employee_id" });
            DropIndex("dbo.employee_photo", new[] { "employee_id" });
            DropIndex("dbo.employee_right", new[] { "employee_id" });
            DropIndex("dbo.employee_right", new[] { "right_id" });
            DropPrimaryKey("dbo.employee_login");
            DropPrimaryKey("dbo.employee_photo");
            DropPrimaryKey("dbo.employee_right");
            AddColumn("dbo.employee_login", "employee_login_id", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.employee_photo", "employee_photo_id", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.employee_right", "employee_right_id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.employee_photo", "photo_systemtype", c => c.String(nullable: false, maxLength: 20));
            AddPrimaryKey("dbo.employee_login", "employee_login_id");
            AddPrimaryKey("dbo.employee_photo", "employee_photo_id");
            AddPrimaryKey("dbo.employee_right", "employee_right_id");
            CreateIndex("dbo.employee_login", new[] { "employee_id", "domain_login" }, unique: true, name: "UIX_EMPLOYEE_LOGIN");
            CreateIndex("dbo.employee_photo", new[] { "employee_id", "photo_systemtype" }, unique: true, name: "UIX_EMPLOYEE_PHOTO");
            CreateIndex("dbo.employee_right", new[] { "employee_id", "right_id" }, unique: true, name: "UIX_EMPLOYEE_RIGHT");
        }
        
        public override void Down()
        {
            DropIndex("dbo.employee_right", "UIX_EMPLOYEE_RIGHT");
            DropIndex("dbo.employee_photo", "UIX_EMPLOYEE_PHOTO");
            DropIndex("dbo.employee_login", "UIX_EMPLOYEE_LOGIN");
            DropPrimaryKey("dbo.employee_right");
            DropPrimaryKey("dbo.employee_photo");
            DropPrimaryKey("dbo.employee_login");
            AlterColumn("dbo.employee_photo", "photo_systemtype", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.employee_right", "employee_right_id");
            DropColumn("dbo.employee_photo", "employee_photo_id");
            DropColumn("dbo.employee_login", "employee_login_id");
            DropTable("dbo.stuffing");
            AddPrimaryKey("dbo.employee_right", new[] { "employee_id", "right_id" });
            AddPrimaryKey("dbo.employee_photo", new[] { "employee_id", "photo_systemtype" });
            AddPrimaryKey("dbo.employee_login", new[] { "employee_id", "domain_login" });
            CreateIndex("dbo.employee_right", "right_id");
            CreateIndex("dbo.employee_right", "employee_id");
            CreateIndex("dbo.employee_photo", "employee_id");
            CreateIndex("dbo.employee_login", "employee_id");
        }
    }
}
