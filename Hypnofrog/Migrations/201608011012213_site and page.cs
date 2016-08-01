namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class siteandpage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        PageId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        Title = c.String(),
                        Color = c.String(),
                        TemplateType = c.String(),
                        HasComments = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PageId);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        SiteId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Iscomplited = c.Boolean(nullable: false),
                        MenuType = c.String(),
                        MenuId = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sites");
            DropTable("dbo.Pages");
        }
    }
}
