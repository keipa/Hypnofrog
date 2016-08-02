namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OneToMany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pages", "SiteId", c => c.Int());
            CreateIndex("dbo.Pages", "SiteId");
            AddForeignKey("dbo.Pages", "SiteId", "dbo.Sites", "SiteId");
            DropColumn("dbo.Pages", "Site");
            DropColumn("dbo.Sites", "MenuId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "MenuId", c => c.Int(nullable: false));
            AddColumn("dbo.Pages", "Site", c => c.Int(nullable: false));
            DropForeignKey("dbo.Pages", "SiteId", "dbo.Sites");
            DropIndex("dbo.Pages", new[] { "SiteId" });
            DropColumn("dbo.Pages", "SiteId");
        }
    }
}
