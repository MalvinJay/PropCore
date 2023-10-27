using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropCore.Domain.Common;
using PropCore.Domain.Entities;
using PropCore.Infrastructure.Identity;
using PropCore.Infrastructure.Messaging.Outbox;

namespace PropCore.Infrastructure.Persistence;

public sealed class PropCoreDbContext(
    DbContextOptions<PropCoreDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Lease> Leases => Set<Lease>();
    public DbSet<RentCharge> RentCharges => Set<RentCharge>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<MaintenanceComment> MaintenanceComments => Set<MaintenanceComment>();
    public DbSet<MaintenanceCost> MaintenanceCosts => Set<MaintenanceCost>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<InspectionItem> InspectionItems => Set<InspectionItem>();
    public DbSet<InspectionPhoto> InspectionPhotos => Set<InspectionPhoto>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(PropCoreDbContext).Assembly);

        builder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Type).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.HasIndex(x => x.ProcessedOn);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
        {
            var aggregate = entry.Entity;

            if (aggregate.DomainEvents.Count > 0)
            {
                foreach (var domainEvent in aggregate.DomainEvents)
                {
                    OutboxMessages.Add(new OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = domainEvent.GetType().FullName!,
                        Payload = System.Text.Json.JsonSerializer.Serialize(
                            domainEvent,
                            domainEvent.GetType()),
                        OccurredOn = domainEvent.OccurredOn
                    });
                }

                aggregate.ClearDomainEvents();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}