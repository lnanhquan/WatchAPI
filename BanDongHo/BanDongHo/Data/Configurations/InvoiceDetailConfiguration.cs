using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchAPI.Models.Entities;

namespace WatchAPI.Data.Configurations;

public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Invoice)
               .WithMany(i => i.InvoiceDetails)
               .HasForeignKey(e => e.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Watch)
               .WithMany()
               .HasForeignKey(e => e.WatchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Quantity).IsRequired();

        builder.Property(e => e.Price).IsRequired();

        builder.Ignore(e => e.Total);
    }
}
