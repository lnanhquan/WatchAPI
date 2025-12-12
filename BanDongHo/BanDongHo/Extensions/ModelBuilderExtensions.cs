using Microsoft.EntityFrameworkCore;
using WatchAPI.Models.Base;

namespace WatchAPI.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAuditableConfiguration(this ModelBuilder modelBuilder)
    {
        var auditableTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => typeof(AuditableEntity).IsAssignableFrom(t.ClrType));

        foreach (var type in auditableTypes)
        {
            var entity = modelBuilder.Entity(type.ClrType);
            entity.Property("CreatedAt").IsRequired();
            entity.Property("Version").IsRequired();
            entity.HasIndex("IsDeleted");
        }
    }
}
