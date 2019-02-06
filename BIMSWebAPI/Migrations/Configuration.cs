namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BIMSWebAPI.Models.BimsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BIMSWebAPI.Models.BimsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            context.Users.AddOrUpdate(
              new Models.User { FirstName = "Aljon", MiddleName = "Agsangre", LastName = "Alacapa", Role = "Administrator", Username = "admin", Password = "admin", CreatedBy = 1, ModifiedBy = 1 }
            );

            context.SecretQuestions.AddOrUpdate(
                new Models.SecretQuestion { Question = "What was your childhood nickname?" },
                new Models.SecretQuestion { Question = "What is the name of your favorite childhood friend?" },
                new Models.SecretQuestion { Question = "In what city or town did your mother and father meet?" },
                new Models.SecretQuestion { Question = "What is the middle name of your oldest child?" },
                new Models.SecretQuestion { Question = "What is the first name of the boy or girl that you first kissed?" },
                new Models.SecretQuestion { Question = "What was the make and model of your first car?" }
            );
        }
    }
}
