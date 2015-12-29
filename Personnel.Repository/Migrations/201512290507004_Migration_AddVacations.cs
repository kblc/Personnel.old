namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddVacations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.vacation",
                c => new
                    {
                        vacation_id = c.Long(nullable: false, identity: true),
                        employee_id = c.Long(nullable: false),
                        vacation_type_id = c.Long(nullable: false),
                        vacation_level_id = c.Long(nullable: false),
                        begin = c.DateTime(nullable: false),
                        day_count = c.Long(nullable: false),
                        not_used = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.vacation_id)
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: true)
                .ForeignKey("dbo.vacation_level", t => t.vacation_level_id, cascadeDelete: true)
                .ForeignKey("dbo.vacation_type", t => t.vacation_type_id, cascadeDelete: true)
                .Index(t => t.employee_id, name: "UIX_EMPLOYEE_VACATION")
                .Index(t => t.vacation_type_id)
                .Index(t => t.vacation_level_id);
            
            CreateTable(
                "dbo.vacation_level",
                c => new
                    {
                        vacation_level_id = c.Long(nullable: false, identity: true),
                        system_name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.vacation_level_id)
                .Index(t => t.system_name, unique: true, name: "UIX_VOCATION_LEVEL_TYPE_SYSTEMNAME");
            
            CreateTable(
                "dbo.vacation_type",
                c => new
                    {
                        vacation_type_id = c.Long(nullable: false, identity: true),
                        system_name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.vacation_type_id)
                .Index(t => t.system_name, unique: true, name: "UIX_VOCATION_TYPE_SYSTEMNAME");
            
            CreateTable(
                "dbo.vacation_agreement",
                c => new
                    {
                        vacation_agreement_id = c.Long(nullable: false, identity: true),
                        vocation_id = c.Long(nullable: false),
                        employee_id = c.Long(nullable: false),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.vacation_agreement_id)
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: false)
                .ForeignKey("dbo.vacation", t => t.vocation_id, cascadeDelete: true)
                .Index(t => new { t.vocation_id, t.employee_id }, unique: true, name: "UIX_VOCATION_AGREEMENT_EMPLOYEE_VOCATION");
            
            CreateTable(
                "dbo.vacation_balance",
                c => new
                    {
                        vacation_balance_id = c.Long(nullable: false, identity: true),
                        employee_id = c.Long(nullable: false),
                        vacation_type_id = c.Long(nullable: false),
                        day_count = c.Long(nullable: false),
                        record_updated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.vacation_balance_id)
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: true)
                .ForeignKey("dbo.vacation_type", t => t.vacation_type_id, cascadeDelete: true)
                .Index(t => t.employee_id, name: "UIX_EMPLOYEE_VACATION")
                .Index(t => t.vacation_type_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.vacation_balance", "vacation_type_id", "dbo.vacation_type");
            DropForeignKey("dbo.vacation_balance", "employee_id", "dbo.employee");
            DropForeignKey("dbo.vacation_agreement", "vocation_id", "dbo.vacation");
            DropForeignKey("dbo.vacation_agreement", "employee_id", "dbo.employee");
            DropForeignKey("dbo.vacation", "vacation_type_id", "dbo.vacation_type");
            DropForeignKey("dbo.vacation", "vacation_level_id", "dbo.vacation_level");
            DropForeignKey("dbo.vacation", "employee_id", "dbo.employee");
            DropIndex("dbo.vacation_balance", new[] { "vacation_type_id" });
            DropIndex("dbo.vacation_balance", "UIX_EMPLOYEE_VACATION");
            DropIndex("dbo.vacation_agreement", "UIX_VOCATION_AGREEMENT_EMPLOYEE_VOCATION");
            DropIndex("dbo.vacation_type", "UIX_VOCATION_TYPE_SYSTEMNAME");
            DropIndex("dbo.vacation_level", "UIX_VOCATION_LEVEL_TYPE_SYSTEMNAME");
            DropIndex("dbo.vacation", new[] { "vacation_level_id" });
            DropIndex("dbo.vacation", new[] { "vacation_type_id" });
            DropIndex("dbo.vacation", "UIX_EMPLOYEE_VACATION");
            DropTable("dbo.vacation_balance");
            DropTable("dbo.vacation_agreement");
            DropTable("dbo.vacation_type");
            DropTable("dbo.vacation_level");
            DropTable("dbo.vacation");
        }
    }
}
