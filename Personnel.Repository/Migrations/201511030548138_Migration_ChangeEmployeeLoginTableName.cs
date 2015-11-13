namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeEmployeeLoginTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EmployeeLogins", newName: "employee_login");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.employee_login", newName: "EmployeeLogins");
        }
    }
}
