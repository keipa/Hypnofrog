namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Avatars",
                c => new
                    {
                        AvatarId = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.AvatarId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Avatars");
        }
    }
}
