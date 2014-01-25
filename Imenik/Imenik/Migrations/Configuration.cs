namespace Imenik.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Imenik.Entities;
    using System.Collections.Generic;
    internal sealed class Configuration : DbMigrationsConfiguration<Imenik.Entities.AdresarContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Imenik.Entities.AdresarContext context)
        {


        }
    }
}
