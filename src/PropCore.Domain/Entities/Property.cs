using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;

namespace PropCore.Domain.Entities;

public sealed class Property : AggregateRoot
{
    private readonly List<Unit> _units = [];

    private Property()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid? OwnerId { get; private set; }
    public Guid AddressId { get; private set; }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public PropertyType PropertyType { get; private set; }
    public PropertyStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Unit> Units => _units.AsReadOnly();

    public static Property Create(
        Guid organizationId,
        string name,
        string? description,
        PropertyType propertyType,
        Guid addressId,
        Guid? ownerId = null)
    {
        var property = new Property
        {
            OrganizationId = organizationId,
            Name = name,
            Description = description,
            PropertyType = propertyType,
            AddressId = addressId,
            OwnerId = ownerId,
            Status = PropertyStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        property.RaiseDomainEvent(new PropertyCreatedDomainEvent(property.Id, organizationId));

        return property;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = PropertyStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PropertyActivatedDomainEvent(Id));
    }

    public void Deactivate()
    {
        Status = PropertyStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }
}