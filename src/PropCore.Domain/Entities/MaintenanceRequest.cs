using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;

namespace PropCore.Domain.Entities;

public sealed class MaintenanceRequest : AggregateRoot
{
    private readonly List<MaintenanceComment> _comments = [];
    private readonly List<MaintenanceCost> _costs = [];

    private MaintenanceRequest()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid TenantId { get; private set; }

    public Guid? AssignedToUserId { get; private set; }

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public MaintenanceCategory Category { get; private set; }
    public MaintenancePriority Priority { get; private set; }
    public MaintenanceStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    public IReadOnlyCollection<MaintenanceComment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<MaintenanceCost> Costs => _costs.AsReadOnly();

    public static MaintenanceRequest Create(
        Guid organizationId,
        Guid propertyId,
        Guid unitId,
        Guid tenantId,
        string title,
        string description,
        MaintenanceCategory category,
        MaintenancePriority priority)
    {
        var request = new MaintenanceRequest
        {
            OrganizationId = organizationId,
            PropertyId = propertyId,
            UnitId = unitId,
            TenantId = tenantId,
            Title = title,
            Description = description,
            Category = category,
            Priority = priority,
            Status = MaintenanceStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        request.RaiseDomainEvent(new MaintenanceRequestCreatedDomainEvent(request.Id, organizationId));

        return request;
    }

    public void AssignTo(Guid userId)
    {
        if (Status != MaintenanceStatus.Submitted && Status != MaintenanceStatus.Assigned)
        {
            throw new InvalidOperationException("Request must be submitted or assigned to be reassigned.");
        }

        AssignedToUserId = userId;
        Status = MaintenanceStatus.Assigned;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new MaintenanceAssignedDomainEvent(Id, userId));
    }

    public void Start()
    {
        if (Status != MaintenanceStatus.Assigned)
        {
            throw new InvalidOperationException("Request must be assigned before it can start.");
        }

        Status = MaintenanceStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != MaintenanceStatus.InProgress)
        {
            throw new InvalidOperationException("Request must be in progress before it can be completed.");
        }

        Status = MaintenanceStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new MaintenanceCompletedDomainEvent(Id));
    }

    public void Verify()
    {
        if (Status != MaintenanceStatus.Completed)
        {
            throw new InvalidOperationException("Only completed requests can be verified.");
        }

        Status = MaintenanceStatus.Verified;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status != MaintenanceStatus.Verified && Status != MaintenanceStatus.Completed)
        {
            throw new InvalidOperationException("Only verified or completed requests can be closed.");
        }

        Status = MaintenanceStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddComment(MaintenanceComment comment)
    {
        _comments.Add(comment);
    }

    public void AddCost(MaintenanceCost cost)
    {
        _costs.Add(cost);
        UpdatedAt = DateTime.UtcNow;
    }
}