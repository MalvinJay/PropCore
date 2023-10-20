using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;

namespace PropCore.Domain.Entities;

public sealed class Inspection : AggregateRoot
{
    private readonly List<InspectionItem> _items = [];

    private Inspection()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid InspectorId { get; private set; }

    public InspectionType Type { get; private set; }

    public DateTime ScheduledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public InspectionStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<InspectionItem> Items => _items.AsReadOnly();

    public static Inspection Create(
        Guid organizationId,
        Guid propertyId,
        Guid unitId,
        Guid inspectorId,
        InspectionType type,
        DateTime scheduledAt)
    {
        return new Inspection
        {
            OrganizationId = organizationId,
            PropertyId = propertyId,
            UnitId = unitId,
            InspectorId = inspectorId,
            Type = type,
            ScheduledAt = scheduledAt,
            Status = InspectionStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(InspectionItem item)
    {
        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? notes = null)
    {
        if (Status != InspectionStatus.Scheduled && Status != InspectionStatus.InProgress)
        {
            throw new InvalidOperationException("Only scheduled or in-progress inspections can be completed.");
        }

        Status = InspectionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new InspectionCompletedDomainEvent(Id));
    }
}