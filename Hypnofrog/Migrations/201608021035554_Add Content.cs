namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contents",
                c => new
                    {
                        ContentId = c.Int(nullable: false, identity: true),
                        HtmlContent = c.String(),
                        Page = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ContentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Contents");
        }
    }
}
