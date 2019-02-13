using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MySql.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace BIMSWebAPI.Models
{
    // Code-Based Configuration and Dependency resolution
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BimsContext : DbContext
    {

        //Add your Dbsets here
        public DbSet<User> Users { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<InventoryMovement> InventoryMovement { get; set; }
        public DbSet<DispenseTransaction> DispenseTransactions { get; set; }
        public DbSet<SecretQuestion> SecretQuestions { get; set; }
        public DbSet<IndigencyTransaction> IndigencyTransactions { get; set; }
        public DbSet<BarangayClearanceTransaction> BarangayClearanceTransactions { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessClearance> BusinessClearanceTransactions { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }


        //Reference the name of your connection string
        public BimsContext() : base("BIMSDb")
        {

        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();

        }

        //public override int SaveChanges()
        //{
        //    AddTimestamps();
        //    return base.SaveChanges();
        //}

        public override async Task<int> SaveChangesAsync()
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            //var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    //((BaseEntity)entity.Entity).DateCreated = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.Now;
                    //((BaseEntity)entity.Entity).CreatedBy = currentUsername;
                }

                //((BaseEntity)entity.Entity).DateModified = DateTime.UtcNow;
                ((BaseEntity)entity.Entity).DateModified = DateTime.Now;
                //((BaseEntity)entity.Entity).ModifiedBy = currentUsername;
            }
        }
    }
}