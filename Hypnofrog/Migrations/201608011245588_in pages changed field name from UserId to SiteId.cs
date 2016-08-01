namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inpageschangedfieldnamefromUserIdtoSiteId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pages", "SiteId", c => c.String());
            DropColumn("dbo.Pages", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pages", "UserId", c => c.String());
            DropColumn("dbo.Pages", "SiteId");
        }
    }
}
