using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;

namespace PropCore.Domain.Entities;

public sealed class Organization : AggregateRoot
{
    private Organization()
    {
    }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public OrganizationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Organization Create(string name, string slug)
    {
        var organization = new Organization
        {
            Name = name,
            Slug = slug,
            Status = OrganizationStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        organization.RaiseDomainEvent(new OrganizationCreatedDomainEvent(organization.Id));

        return organization;
    }

    public void Rename(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = OrganizationStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}