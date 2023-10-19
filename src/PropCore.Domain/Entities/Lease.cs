using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;
using PropCore.Domain.ValueObjects;

namespace PropCore.Domain.Entities;

public sealed class Lease : AggregateRoot
{
    private Lease()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid TenantId { get; private set; }

    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public Money MonthlyRent { get; private set; } = null!;
    public Money SecurityDeposit { get; private set; } = null!;

    public LeaseStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    public static Lease Create(
        Guid organizationId,
        Guid unitId,
        Guid tenantId,
        DateOnly startDate,
        DateOnly endDate,
        Money monthlyRent,
        Money securityDeposit)
    {
        if (endDate <= startDate)
        {
            throw new InvalidOperationException("Lease end date must be after start date.");
        }

        var lease = new Lease
        {
            OrganizationId = organizationId,
            UnitId = unitId,
            TenantId = tenantId,
            StartDate = startDate,
            EndDate = endDate,
            MonthlyRent = monthlyRent,
            SecurityDeposit = securityDeposit,
            Status = LeaseStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        lease.RaiseDomainEvent(new LeaseCreatedDomainEvent(lease.Id, organizationId));

        return lease;
    }

    public void Submit()
    {
        if (Status != LeaseStatus.Draft)
        {
            throw new InvalidOperationException("Only draft leases can be submitted for approval.");
        }

        Status = LeaseStatus.PendingApproval;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeaseSubmittedDomainEvent(Id));
    }

    public void Approve()
    {
        if (Status != LeaseStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only pending leases can be approved.");
        }

        RaiseDomainEvent(new LeaseApprovedDomainEvent(Id));
    }

    public void Activate()
    {
        if (Status != LeaseStatus.PendingApproval && Status != LeaseStatus.Draft)
        {
            throw new InvalidOperationException("Only approved leases can be activated.");
        }

        if (Status == LeaseStatus.Expired)
        {
            throw new InvalidOperationException("Expired leases cannot be activated.");
        }

        Status = LeaseStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeaseActivatedDomainEvent(Id));
    }

    public void MarkExpiring()
    {
        if (Status != LeaseStatus.Active)
        {
            throw new InvalidOperationException("Only active leases can expire.");
        }

        Status = LeaseStatus.Expiring;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkExpired()
    {
        if (Status != LeaseStatus.Active && Status != LeaseStatus.Expiring)
        {
            throw new InvalidOperationException("Only active or expiring leases can expire.");
        }

        Status = LeaseStatus.Expired;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeaseExpiredDomainEvent(Id));
    }

    public void Terminate()
    {
        if (Status != LeaseStatus.Active)
        {
            throw new InvalidOperationException("Only active leases can be terminated.");
        }

        Status = LeaseStatus.Terminated;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeaseTerminatedDomainEvent(Id));
    }

    public void Renew(DateOnly newEndDate, Money newMonthlyRent)
    {
        if (Status != LeaseStatus.Active && Status != LeaseStatus.Expiring)
        {
            throw new InvalidOperationException("Only active or expiring leases can be renewed.");
        }

        if (newEndDate <= EndDate)
        {
            throw new InvalidOperationException("Renewed end date must be after the current end date.");
        }

        EndDate = newEndDate;
        MonthlyRent = newMonthlyRent;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LeaseRenewedDomainEvent(Id));
    }
}