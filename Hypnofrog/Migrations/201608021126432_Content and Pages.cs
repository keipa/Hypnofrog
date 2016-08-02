namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContentandPages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contents", "PageId", c => c.Int());
            CreateIndex("dbo.Contents", "PageId");
            AddForeignKey("dbo.Contents", "PageId", "dbo.Pages", "PageId");
            DropColumn("dbo.Contents", "Page");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contents", "Page", c => c.Int(nullable: false));
            DropForeignKey("dbo.Contents", "PageId", "dbo.Pages");
            DropIndex("dbo.Contents", new[] { "PageId" });
            DropColumn("dbo.Contents", "PageId");
        }
    }
}
