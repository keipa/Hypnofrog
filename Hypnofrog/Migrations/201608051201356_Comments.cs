namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        UserId = c.String(),
                        SiteId = c.Int(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Sites", t => t.SiteId)
                .Index(t => t.SiteId);
            
            AddColumn("dbo.Sites", "HasComments", c => c.Boolean(nullable: false));
            DropColumn("dbo.Pages", "HasComments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pages", "HasComments", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Comments", "SiteId", "dbo.Sites");
            DropIndex("dbo.Comments", new[] { "SiteId" });
            DropColumn("dbo.Sites", "HasComments");
            DropTable("dbo.Comments");
        }
    }
}
