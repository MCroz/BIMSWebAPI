using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MySql.Data.Entity;
using System.Threading.Tasks;
using BIMSWebAPI.Models;
using System.Data.Entity.Infrastructure;

namespace BIMSWebAPI.App_Code
{
    public static class AuditLogHelper
    {
        public static int GenerateUpdateLog(BimsContext context, int UserID)
        {
            var modifiedEntities = context.ChangeTracker.Entries()
            .Where(p => p.State == EntityState.Modified && p.Entity.GetType().Name != "ChangeLog").ToList();
            //var now = DateTime.UtcNow;
            var now = DateTime.Now;
            foreach (var change in modifiedEntities)
            {
                var entityName = change.Entity.GetType().Name;
                var primaryKey = GetPrimaryKeyValue(change, context);

                foreach (var prop in change.OriginalValues.PropertyNames)
                {
                    var originalValue = change.OriginalValues[prop].ToString();
                    var currentValue = change.CurrentValues[prop].ToString();
                    if (originalValue != currentValue)
                    {
                        //Create New Log
                    }
                }
            }

            return context.SaveChanges();
        }

        static object GetPrimaryKeyValue(DbEntityEntry entry, BimsContext thisContext)
        {
            var objectStateEntry = ((IObjectContextAdapter)thisContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }
    }
}