using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropCore.Domain.Entities;

namespace PropCore.Infrastructure.Persistence.Configurations;

internal sealed class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        builder.ToTable("MaintenanceRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.UnitId);
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.MaintenanceRequestId);

        builder.HasMany(x => x.Costs)
            .WithOne()
            .HasForeignKey(x => x.MaintenanceRequestId);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}

internal sealed class MaintenanceCommentConfiguration : IEntityTypeConfiguration<MaintenanceComment>
{
    public void Configure(EntityTypeBuilder<MaintenanceComment> builder)
    {
        builder.ToTable("MaintenanceComments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .HasMaxLength(2000)
            .IsRequired();
    }
}

internal sealed class MaintenanceCostConfiguration : IEntityTypeConfiguration<MaintenanceCost>
{
    public void Configure(EntityTypeBuilder<MaintenanceCost> builder)
    {
        builder.ToTable("MaintenanceCosts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);
    }
}