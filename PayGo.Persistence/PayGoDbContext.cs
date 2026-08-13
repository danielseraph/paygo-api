using Microsoft.EntityFrameworkCore;
using PayGo.Model.Entities;
using PayGo.Model.Entities.Common;
namespace PayGo.Persistence;
// Represents the database context for the PayGo application
public class PayGoDbContext : DbContext
{
    public PayGoDbContext(DbContextOptions<PayGoDbContext> options) : base(options)
    {
    }
    public DbSet<Merchant> Merchants => Set<Merchant>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<PaymentLink> PaymentLinks => Set<PaymentLink>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ReconciliationRecord> ReconciliationRecords => Set<ReconciliationRecord>();
    public DbSet<WebhookEvent> WebhookEvents => Set<WebhookEvent>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure entity relationships and constraints here if needed
        modelBuilder.Entity<Merchant>()
            .HasMany(m => m.Transactions)
            .WithOne(t => t.Merchant)
            .HasForeignKey(t => t.MerchantId);
        modelBuilder.Entity<Merchant>()
            .HasMany(m => m.PaymentLinks)
            .WithOne(pl => pl.Merchant)
            .HasForeignKey(pl => pl.MerchantId);
        modelBuilder.Entity<Merchant>()
            .HasMany(m => m.LedgerEntries)
            .WithOne(le => le.Merchant)
            .HasForeignKey(le => le.MerchantId);
        modelBuilder.Entity<Merchant>()
            .HasMany(m => m.Notifications)
            .WithOne(n => n.Merchant)
            .HasForeignKey(n => n.MerchantId);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

}

