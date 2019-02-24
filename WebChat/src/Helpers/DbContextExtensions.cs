using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebChat.Database;

namespace WebChat.Helpers
{
    public static class DbContextExtensions
    {
        public static void ApplySnakeCase(this ChatDbContext context, ModelBuilder builder)
        {
            foreach(var entity in builder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.Relational().TableName = (entity.Name.Split(".").Last() + (entity.Name.EndsWith("s") ? "es" : "s")).ToSnakeCase();

                // Replace column names            
                foreach(var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = property.Name.ToSnakeCase();
                }

                foreach(var key in entity.GetKeys())
                {
                    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                }

                foreach(var key in entity.GetForeignKeys())
                {
                    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                }

                foreach(var index in entity.GetIndexes())
                {
                    index.Relational().Name = index.Relational().Name.ToSnakeCase();
                }
            }
        }
    }
}