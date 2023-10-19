using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;
using PropCore.Domain.ValueObjects;

namespace PropCore.Domain.Entities;

public sealed class RentCharge : AggregateRoot
{
    private RentCharge()
    {
    }

    public Guid LeaseId { get; private set; }

    public Money Amount { get; private set; } = null!;
    public DateOnly DueDate { get; private set; }

    public RentChargeStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static RentCharge Create(
        Guid leaseId,
        Money amount,
        DateOnly dueDate)
    {
        var charge = new RentCharge
        {
            LeaseId = leaseId,
            Amount = amount,
            DueDate = dueDate,
            Status = RentChargeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        charge.RaiseDomainEvent(new RentChargeCreatedDomainEvent(charge.Id, leaseId));

        return charge;
    }

    public void MarkDue()
    {
        Status = RentChargeStatus.Due;
    }

    public void MarkPaid()
    {
        Status = RentChargeStatus.Paid;
    }

    public void MarkOverdue()
    {
        Status = RentChargeStatus.Overdue;
    }
}