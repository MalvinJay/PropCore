using PropCore.Domain.Common;

namespace PropCore.Domain.Events;

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}

public sealed record OrganizationCreatedDomainEvent(Guid OrganizationId) : DomainEvent;

public sealed record PropertyCreatedDomainEvent(Guid PropertyId, Guid OrganizationId) : DomainEvent;
public sealed record PropertyActivatedDomainEvent(Guid PropertyId) : DomainEvent;

public sealed record UnitCreatedDomainEvent(Guid UnitId, Guid PropertyId) : DomainEvent;

public sealed record LeaseCreatedDomainEvent(Guid LeaseId, Guid OrganizationId) : DomainEvent;
public sealed record LeaseSubmittedDomainEvent(Guid LeaseId) : DomainEvent;
public sealed record LeaseApprovedDomainEvent(Guid LeaseId) : DomainEvent;
public sealed record LeaseActivatedDomainEvent(Guid LeaseId) : DomainEvent;
public sealed record LeaseExpiredDomainEvent(Guid LeaseId) : DomainEvent;
public sealed record LeaseTerminatedDomainEvent(Guid LeaseId) : DomainEvent;
public sealed record LeaseRenewedDomainEvent(Guid LeaseId) : DomainEvent;

public sealed record RentChargeCreatedDomainEvent(Guid RentChargeId, Guid LeaseId) : DomainEvent;

public sealed record PaymentRecordedDomainEvent(Guid PaymentId, Guid LeaseId) : DomainEvent;
public sealed record PaymentCompletedDomainEvent(Guid PaymentId, Guid LeaseId) : DomainEvent;
public sealed record PaymentFailedDomainEvent(Guid PaymentId, Guid LeaseId) : DomainEvent;

public sealed record MaintenanceRequestCreatedDomainEvent(Guid MaintenanceRequestId, Guid OrganizationId) : DomainEvent;
public sealed record MaintenanceAssignedDomainEvent(Guid MaintenanceRequestId, Guid AssignedToUserId) : DomainEvent;
public sealed record MaintenanceCompletedDomainEvent(Guid MaintenanceRequestId) : DomainEvent;

public sealed record InspectionCompletedDomainEvent(Guid InspectionId) : DomainEvent;

public sealed record DocumentUploadedDomainEvent(Guid DocumentId, Guid OrganizationId) : DomainEvent;