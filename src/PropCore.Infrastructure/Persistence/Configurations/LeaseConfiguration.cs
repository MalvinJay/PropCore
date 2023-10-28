using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropCore.Domain.Entities;
using PropCore.Domain.ValueObjects;

namespace PropCore.Infrastructure.Persistence.Configurations;

internal sealed class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> builder)
    {
        builder.ToTable("Leases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MonthlyRent)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);

        builder.Property(x => x.SecurityDeposit)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.UnitId);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.OrganizationId);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}