using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;

public class PaymentLinkConfiguration : IEntityTypeConfiguration<PaymentLink>
{
    public void Configure(EntityTypeBuilder<PaymentLink> builder)
    {
        builder.ToTable("PaymentLinks");

        builder.HasKey(pl => pl.Id);

        builder.Property(pl => pl.Reference).IsRequired().HasMaxLength(100);
        builder.Property(pl => pl.Title).IsRequired().HasMaxLength(200);
        builder.Property(pl => pl.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(pl => pl.Amount).HasColumnType("decimal(18,4)");

        builder.HasIndex(pl => pl.Reference).IsUnique();

        builder.HasOne(pl => pl.Merchant)
            .WithMany(m => m.PaymentLinks)
            .HasForeignKey(pl => pl.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(pl => !pl.IsDeleted);
    }
}