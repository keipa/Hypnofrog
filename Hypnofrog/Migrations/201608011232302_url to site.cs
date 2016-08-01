namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class urltosite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "Url");
        }
    }
}
