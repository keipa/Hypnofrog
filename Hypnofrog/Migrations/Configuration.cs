namespace Hypnofrog.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Hypnofrog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Hypnofrog.DBModels.Context";
        }

        protected override void Seed(Hypnofrog.Models.ApplicationDbContext context)
        {
            
        }
    }
}
