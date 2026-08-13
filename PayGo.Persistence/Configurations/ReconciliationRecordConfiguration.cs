using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;

public class ReconciliationRecordConfiguration : IEntityTypeConfiguration<ReconciliationRecord>
{
    public void Configure(EntityTypeBuilder<ReconciliationRecord> builder)
    {
        builder.ToTable("ReconciliationRecords");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ProviderReference).HasMaxLength(100);
        builder.Property(r => r.ExpectedAmount).HasColumnType("decimal(18,4)");
        builder.Property(r => r.ActualAmount).HasColumnType("decimal(18,4)");

        builder.HasOne(r => r.Merchant)
            .WithMany() // Single navigation is fine here
            .HasForeignKey(r => r.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Transaction)
            .WithMany()
            .HasForeignKey(r => r.TransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}