using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchAPI.Models.Entities;

namespace WatchAPI.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.User)
               .WithMany(u => u.CartItems)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Watch)
               .WithMany()
               .HasForeignKey(e => e.WatchId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Quantity).IsRequired();

        builder.Ignore(e => e.Total);
    }
}
