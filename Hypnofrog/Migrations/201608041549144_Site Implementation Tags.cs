namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SiteImplementationTags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "Tags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "Tags");
        }
    }
}
