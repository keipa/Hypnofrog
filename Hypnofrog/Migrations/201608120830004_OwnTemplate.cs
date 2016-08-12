namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OwnTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OwnTemplates",
                c => new
                    {
                        OwnTemplateId = c.Int(nullable: false, identity: true),
                        HtmlRealize = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        PageId = c.Int(),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.OwnTemplateId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OwnTemplates");
        }
    }
}
