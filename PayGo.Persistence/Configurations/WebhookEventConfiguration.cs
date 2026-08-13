using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayGo.Model.Entities;

namespace PayGo.Persistence.Configurations;

public class WebhookEventConfiguration : IEntityTypeConfiguration<WebhookEvent>
{
    public void Configure(EntityTypeBuilder<WebhookEvent> builder)
    {
        builder.ToTable("WebhookEvents");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Reference).IsRequired().HasMaxLength(100);
        builder.Property(w => w.EventType).IsRequired().HasMaxLength(100);
        builder.Property(w => w.ProviderEventId).HasMaxLength(150);

        // Map payload to Postgres JSONB for querying/efficiency
        builder.Property(w => w.PayloadJson).HasColumnType("jsonb").IsRequired();

        builder.HasIndex(w => w.ProviderEventId);

        builder.HasQueryFilter(w => !w.IsDeleted);
    }
}