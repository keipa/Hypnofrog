namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldtypechangedtointinPagespageidandsiteid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Pages");
            AlterColumn("dbo.Pages", "PageId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Pages", "SiteId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Pages", "PageId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Pages");
            AlterColumn("dbo.Pages", "SiteId", c => c.String());
            AlterColumn("dbo.Pages", "PageId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Pages", "PageId");
        }
    }
}
