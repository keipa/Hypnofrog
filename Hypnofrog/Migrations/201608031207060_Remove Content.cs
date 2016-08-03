namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveContent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contents", "PageId", "dbo.Pages");
            DropIndex("dbo.Contents", new[] { "PageId" });
            AddColumn("dbo.Pages", "FirstContent", c => c.String());
            AddColumn("dbo.Pages", "SecondContent", c => c.String());
            AddColumn("dbo.Pages", "ThirdContent", c => c.String());
            DropTable("dbo.Contents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Contents",
                c => new
                    {
                        ContentId = c.Int(nullable: false, identity: true),
                        HtmlContent = c.String(),
                        PageId = c.Int(),
                    })
                .PrimaryKey(t => t.ContentId);
            
            DropColumn("dbo.Pages", "ThirdContent");
            DropColumn("dbo.Pages", "SecondContent");
            DropColumn("dbo.Pages", "FirstContent");
            CreateIndex("dbo.Contents", "PageId");
            AddForeignKey("dbo.Contents", "PageId", "dbo.Pages", "PageId");
        }
    }
}
