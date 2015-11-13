namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeFileEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.file", "encoding_name", c => c.String(maxLength: 100));
            DropColumn("dbo.file", "file_path");
        }
        
        public override void Down()
        {
            AddColumn("dbo.file", "file_path", c => c.String(nullable: false));
            DropColumn("dbo.file", "encoding_name");
        }
    }
}
