using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;
public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.ToTable("Merchants");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.BusinessName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(150);
        builder.Property(m => m.PhoneNumber).HasMaxLength(11);
        builder.Property(m => m.BusinessAddress).HasMaxLength(500);
        builder.Property(m => m.CurrencyCode).IsRequired().HasMaxLength(3);

        builder.HasIndex(m => m.Email).IsUnique();
        // Query filter for soft delete
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}

