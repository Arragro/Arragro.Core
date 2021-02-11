using Arragro.Core.Common.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Arragro.Core.EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtentions
    {
        public static ModelBuilder SnakeCaseTablesAndProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName().ToSnakeCase();
                // Replace table names
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                // Replace column names            
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(tableName, entity.GetSchema())).ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName(StoreObjectIdentifier.Table(tableName, entity.GetSchema())).ToSnakeCase());
                }
            }

            return modelBuilder;
        }
    }
}
