namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ratingfields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        RateId = c.Int(nullable: false, identity: true),
                        Site = c.String(),
                        User = c.String(),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RateId);
            
            AddColumn("dbo.Sites", "Rate", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "Rate");
            DropTable("dbo.Rates");
        }
    }
}
