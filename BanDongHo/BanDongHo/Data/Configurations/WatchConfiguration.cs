using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchAPI.Models.Entities;

namespace WatchAPI.Data.Configurations
{
    public class WatchConfiguration : IEntityTypeConfiguration<Watch>
    {
        public void Configure(EntityTypeBuilder<Watch> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Price).IsRequired();
            builder.Property(e => e.Category).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Brand).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Description).HasMaxLength(500);
            builder.Property(e => e.ImageUrl).HasMaxLength(500);
        }
    }
}
