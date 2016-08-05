namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentAvatarsSecond : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "UserId", c => c.String());
            DropColumn("dbo.Comments", "UserEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "UserEmail", c => c.String());
            DropColumn("dbo.Comments", "UserId");
        }
    }
}
