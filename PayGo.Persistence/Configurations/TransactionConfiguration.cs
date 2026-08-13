using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Reference).IsRequired().HasMaxLength(100);
        builder.Property(t => t.ProviderReference).HasMaxLength(100);
        builder.Property(t => t.CurrencyCode).IsRequired().HasMaxLength(3);

        // Ensure decimal precision for money
        builder.Property(t => t.Amount).HasColumnType("decimal(18,4)");
        builder.Property(t => t.ProviderFee).HasColumnType("decimal(18,4)");
        builder.Property(t => t.NetAmount).HasColumnType("decimal(18,4)");
        builder.Property(t => t.RiskScore).HasColumnType("decimal(5,2)");

        builder.HasIndex(t => t.Reference).IsUnique();
        builder.HasIndex(t => t.ProviderReference);
        builder.HasIndex(t => t.MerchantId);

        // Relationships
        builder.HasOne(t => t.Merchant)
            .WithMany(m => m.Transactions)
            .HasForeignKey(t => t.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.PaymentLink)
            .WithMany(pl => pl.Transactions)
            .HasForeignKey(t => t.PaymentLinkId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}