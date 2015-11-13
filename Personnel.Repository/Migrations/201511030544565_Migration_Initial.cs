namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.appoint",
                c => new
                    {
                        appoint_id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.appoint_id);
            
            CreateTable(
                "dbo.department",
                c => new
                    {
                        department_id = c.Long(nullable: false, identity: true),
                        parent_department_id = c.Long(),
                        name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.department_id)
                .ForeignKey("dbo.department", t => t.parent_department_id)
                .Index(t => t.parent_department_id);
            
            CreateTable(
                "dbo.EmployeeLogins",
                c => new
                    {
                        employee_id = c.Long(nullable: false),
                        domain_login = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.employee_id, t.domain_login })
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: true)
                .Index(t => t.employee_id);
            
            CreateTable(
                "dbo.employee",
                c => new
                    {
                        employee_id = c.Long(nullable: false, identity: true),
                        surname = c.String(nullable: false, maxLength: 50),
                        name = c.String(nullable: false, maxLength: 50),
                        patronymic = c.String(maxLength: 50),
                        birthday = c.DateTime(),
                        email = c.String(maxLength: 200),
                        phone = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.employee_id);
            
            CreateTable(
                "dbo.employee_photo",
                c => new
                    {
                        employee_id = c.Long(nullable: false),
                        photo_systemtype = c.String(nullable: false, maxLength: 128),
                        width = c.Int(nullable: false),
                        height = c.Int(nullable: false),
                        file_uid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.employee_id, t.photo_systemtype })
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: true)
                .ForeignKey("dbo.file", t => t.file_uid, cascadeDelete: true)
                .Index(t => t.employee_id)
                .Index(t => t.file_uid);
            
            CreateTable(
                "dbo.file",
                c => new
                    {
                        file_uid = c.Guid(nullable: false),
                        file_name = c.String(nullable: false),
                        file_path = c.String(nullable: false),
                        file_size = c.Long(nullable: false),
                        mime = c.String(nullable: false),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.file_uid);
            
            CreateTable(
                "dbo.employee_right",
                c => new
                    {
                        employee_id = c.Long(nullable: false),
                        right_id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.employee_id, t.right_id })
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: true)
                .ForeignKey("dbo.right", t => t.right_id, cascadeDelete: true)
                .Index(t => t.employee_id)
                .Index(t => t.right_id);
            
            CreateTable(
                "dbo.right",
                c => new
                    {
                        right_id = c.Long(nullable: false, identity: true),
                        system_name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.right_id)
                .Index(t => t.system_name, unique: true, name: "UIX_RIGHT_SYSTEMNAME");
            
            CreateTable(
                "dbo.history",
                c => new
                    {
                        history_id = c.Long(nullable: false, identity: true),
                        source_id = c.String(nullable: false, maxLength: 100),
                        date = c.DateTime(nullable: false),
                        sourcetype = c.String(nullable: false),
                        changetype = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.history_id);
            
            CreateTable(
                "dbo.stuffing",
                c => new
                    {
                        stuffing_id = c.Long(nullable: false, identity: true),
                        position = c.Long(nullable: false),
                        department_id = c.Long(nullable: false),
                        appoint_id = c.Long(nullable: false),
                        employee_id = c.Long(),
                    })
                .PrimaryKey(t => t.stuffing_id)
                .ForeignKey("dbo.appoint", t => t.appoint_id, cascadeDelete: true)
                .ForeignKey("dbo.department", t => t.department_id, cascadeDelete: true)
                .ForeignKey("dbo.employee", t => t.employee_id)
                .Index(t => t.department_id)
                .Index(t => t.appoint_id)
                .Index(t => t.employee_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.stuffing", "employee_id", "dbo.employee");
            DropForeignKey("dbo.stuffing", "department_id", "dbo.department");
            DropForeignKey("dbo.stuffing", "appoint_id", "dbo.appoint");
            DropForeignKey("dbo.EmployeeLogins", "employee_id", "dbo.employee");
            DropForeignKey("dbo.employee_right", "right_id", "dbo.right");
            DropForeignKey("dbo.employee_right", "employee_id", "dbo.employee");
            DropForeignKey("dbo.employee_photo", "file_uid", "dbo.file");
            DropForeignKey("dbo.employee_photo", "employee_id", "dbo.employee");
            DropForeignKey("dbo.department", "parent_department_id", "dbo.department");
            DropIndex("dbo.stuffing", new[] { "employee_id" });
            DropIndex("dbo.stuffing", new[] { "appoint_id" });
            DropIndex("dbo.stuffing", new[] { "department_id" });
            DropIndex("dbo.right", "UIX_RIGHT_SYSTEMNAME");
            DropIndex("dbo.employee_right", new[] { "right_id" });
            DropIndex("dbo.employee_right", new[] { "employee_id" });
            DropIndex("dbo.employee_photo", new[] { "file_uid" });
            DropIndex("dbo.employee_photo", new[] { "employee_id" });
            DropIndex("dbo.EmployeeLogins", new[] { "employee_id" });
            DropIndex("dbo.department", new[] { "parent_department_id" });
            DropTable("dbo.stuffing");
            DropTable("dbo.history");
            DropTable("dbo.right");
            DropTable("dbo.employee_right");
            DropTable("dbo.file");
            DropTable("dbo.employee_photo");
            DropTable("dbo.employee");
            DropTable("dbo.EmployeeLogins");
            DropTable("dbo.department");
            DropTable("dbo.appoint");
        }
    }
}
