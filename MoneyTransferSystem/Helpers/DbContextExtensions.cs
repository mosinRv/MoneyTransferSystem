using System.Collections.Generic;
using System.Reflection;
using MoneyTransferSystem.Helpers;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;

namespace MoneyTransferSystem.Helpers
{
    public static class DbContextExtensions
    {
        public static List<PropertyInfo> GetDbSetProperties(this MyDbContext context)
        {
            var dbSetProperties = new List<PropertyInfo>();
            var properties = context.GetType().GetProperties();

            foreach (var property in properties)
            {
                var setType = property.PropertyType;

                var isDbSet = setType.IsGenericType &&
                              (typeof(DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));

                if (isDbSet)
                    dbSetProperties.Add(property);
            }

            return dbSetProperties;
        }

        public static void ApplyOnModelCreatingFromAllEntities(this MyDbContext context, ModelBuilder builder)
        {
            var props = context.GetDbSetProperties();
            foreach (var prop in props)
            {
                var methodInfo = prop.PropertyType.GetGenericArguments()[0].GetMethod("OnModelCreating");
                methodInfo?.Invoke(null, new object[] {builder});
            }
        }

        public static void ApplySnakeCase(this DbContext context, ModelBuilder builder)
        {
            foreach(var entity in builder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                // Replace column names
                foreach(var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToSnakeCase());
                }

                foreach(var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach(var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                foreach(var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.Name.ToSnakeCase());
                }
            }
        }
    }
}