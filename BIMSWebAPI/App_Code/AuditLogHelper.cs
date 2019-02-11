using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MySql.Data.Entity;
using System.Threading.Tasks;
using BIMSWebAPI.Models;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace BIMSWebAPI.App_Code
{
    public static class AuditLogHelper
    {
        public static string FetchChanges(BimsContext context)
        {
            string logChanges = "";

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
                    if (prop != "DateModified" && prop != "ModifiedBy")
                    {
                        var originalValue = change.OriginalValues[prop].ToString();
                        var currentValue = change.CurrentValues[prop].ToString();
                        if (originalValue != currentValue)
                        {
                            //Create New Log
                            if (logChanges != "")
                            {
                                logChanges += ", ";
                            }
                            logChanges += prop + " From: " + originalValue + " To: " + currentValue;
                        }
                    }

                }
            }
            return logChanges;
            //return context.SaveChanges();
        }

        static object GetPrimaryKeyValue(DbEntityEntry entry, BimsContext thisContext)
        {
            var objectStateEntry = ((IObjectContextAdapter)thisContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }

        public static int GenerateLog(BimsContext context, string type, string action)
        {
            SystemLog newLog = new SystemLog
            {
                LogTime = DateTime.Now,
                LogAction = action,
                LogType = type
            };
            context.SystemLogs.Add(newLog);
            return 1;
            //return context.SaveChanges();
        }


        public static string FetchAdded(object myClass, Type type)
        {
            string logChanges = "";
            foreach (PropertyInfo pi in type.GetProperties())
            {
                //pi.GetValue(myClass, null)?.ToString();
                if (logChanges != "")
                {
                    logChanges += ", ";
                }
                logChanges += pi.Name + " = " + pi.GetValue(myClass, null)?.ToString();
            }
            return "";
            //return context.SaveChanges();
        }
    }
}