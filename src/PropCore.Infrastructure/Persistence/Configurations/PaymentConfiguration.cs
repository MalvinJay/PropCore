using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropCore.Domain.Entities;
using PropCore.Domain.ValueObjects;

namespace PropCore.Infrastructure.Persistence.Configurations;

internal sealed class RentChargeConfiguration : IEntityTypeConfiguration<RentCharge>
{
    public void Configure(EntityTypeBuilder<RentCharge> builder)
    {
        builder.ToTable("RentCharges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.LeaseId);
    }
}

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.LeaseId);
        builder.HasIndex(x => x.RentChargeId);

        builder.HasIndex(x => x.Reference)
            .IsUnique()
            .HasFilter("[Reference] IS NOT NULL");
    }
}