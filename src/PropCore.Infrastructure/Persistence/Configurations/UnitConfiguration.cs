using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropCore.Domain.Entities;

namespace PropCore.Infrastructure.Persistence.Configurations;

internal sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.MonthlyRent)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);

        builder.Property(x => x.SecurityDeposit)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);

        builder.HasIndex(x => new { x.PropertyId, x.UnitNumber })
            .IsUnique();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}