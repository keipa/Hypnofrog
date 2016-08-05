namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentAvatars : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "UserEmail", c => c.String());
            AddColumn("dbo.Comments", "UserAvatar", c => c.String());
            DropColumn("dbo.Comments", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "UserId", c => c.String());
            DropColumn("dbo.Comments", "UserAvatar");
            DropColumn("dbo.Comments", "UserEmail");
        }
    }
}
