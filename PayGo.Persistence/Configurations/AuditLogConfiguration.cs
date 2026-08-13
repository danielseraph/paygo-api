using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityId).HasMaxLength(100);

        builder.Property(a => a.OldValuesJson).HasColumnType("jsonb");
        builder.Property(a => a.NewValuesJson).HasColumnType("jsonb");

        // Audit Logs are immutable; we shouldn't allow soft delete filtering either, 
        // they should always be preserved entirely.
    }
}