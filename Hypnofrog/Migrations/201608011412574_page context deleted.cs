namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pagecontextdeleted : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Pages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        PageId = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        Title = c.String(),
                        Color = c.String(),
                        TemplateType = c.String(),
                        HasComments = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PageId);
            
        }
    }
}
