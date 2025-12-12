using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchAPI.Constants;
using WatchAPI.Models.Entities;

namespace WatchAPI.Data.Configurations;

public class WatchConfiguration : IEntityTypeConfiguration<Watch>
{
    public void Configure(EntityTypeBuilder<Watch> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(WatchConstants.MaxNameLength);
        builder.Property(e => e.Price).IsRequired();
        builder.Property(e => e.Category).IsRequired().HasMaxLength(WatchConstants.MaxCategoryLength);
        builder.Property(e => e.Brand).IsRequired().HasMaxLength(WatchConstants.MaxBrandLength);
        builder.Property(e => e.Description).HasMaxLength(WatchConstants.MaxDescriptionLength);
        builder.Property(e => e.ImageUrl).HasMaxLength(WatchConstants.MaxImageUrlLength);
    }
}
