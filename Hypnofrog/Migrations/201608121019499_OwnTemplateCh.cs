namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OwnTemplateCh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OwnTemplates", "UserName", c => c.String());
            DropColumn("dbo.OwnTemplates", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OwnTemplates", "UserId", c => c.Int());
            DropColumn("dbo.OwnTemplates", "UserName");
        }
    }
}
