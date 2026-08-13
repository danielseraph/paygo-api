using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("LedgerEntries");

        builder.HasKey(le => le.Id);

        builder.Property(le => le.Amount).HasColumnType("decimal(18,4)");
        builder.Property(le => le.BalanceAfter).HasColumnType("decimal(18,4)");
        builder.Property(le => le.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(le => le.Reference).HasMaxLength(100);

        builder.HasOne(le => le.Merchant)
            .WithMany(m => m.LedgerEntries)
            .HasForeignKey(le => le.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(le => le.Transaction)
            .WithMany(t => t.LedgerEntries)
            .HasForeignKey(le => le.TransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(le => !le.IsDeleted);
    }
}