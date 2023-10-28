using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropCore.Domain.Entities;

namespace PropCore.Infrastructure.Persistence.Configurations;

internal sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.OrganizationId);

        builder.HasOne<Address>()
            .WithMany()
            .HasForeignKey(x => x.AddressId);

        builder.HasMany(x => x.Units)
            .WithOne()
            .HasForeignKey(x => x.PropertyId);
    }
}