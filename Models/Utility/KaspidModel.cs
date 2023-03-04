
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Kaspid.Models.Utility
{


    public class KaspidModel : DalEntities
    {
        

        public int SaveChanges(string URL)
        {
            //-----------------------------Modified-----------------------------
            var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
            var now = DateTime.UtcNow;

            foreach (var change in modifiedEntities)
            {
                var entityName = change.Entity.GetType().Name;
                var primaryKey = GetPrimaryKeyValue(change);
                SaveLog(URL, entityName, primaryKey.ToString(), EntityState.Modified,"");

            }
            //-----------------------------Insert-----------------------------
            var AddedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();

            //--------------------------Delete----------------------------
            var DeletedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).ToList();
            foreach (var change in DeletedEntities)
            {
                var entityName = change.Entity.GetType().Name;
                var primaryKey = GetPrimaryKeyValue(change);
                SaveLog(URL, entityName, primaryKey.ToString(), EntityState.Deleted,"");
            }

            this.InvalidateSecondLevelCache();//for empty Catch
            int Result = base.SaveChanges();

            foreach (var change in AddedEntities)
            {
                var entityName = change.Entity.GetType().Name;
                var primaryKey = change.CurrentValues.GetValue<object>("Id").ToString();
                SaveLog(URL, entityName, primaryKey.ToString(), EntityState.Added,"");
            }
            return Result;
        }
        public object GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }
        private long SaveLog(string URL, string tableName, string tableId, EntityState _EntityState, string description)
        {
            using (var db = new DalEntities())
            {
                var logOperation = db.LogOperations.Create();
                logOperation.DateX = DateTime.Now;
                logOperation.Description = "";
                logOperation.Operation = (byte)_EntityState;
                logOperation.TableId = tableId;
                logOperation.PageUrl = URL;
                logOperation.TableName = tableName;
                logOperation.UserId = SiteUtility .UserId;
                db.LogOperations.Add(logOperation);
                db.SaveChanges();
                return logOperation.Id;

            }
        }



    }
   
}