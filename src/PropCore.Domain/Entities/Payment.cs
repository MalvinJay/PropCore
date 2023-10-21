using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.Events;
using PropCore.Domain.ValueObjects;

namespace PropCore.Domain.Entities;

public sealed class Payment : AggregateRoot
{
    private Payment()
    {
    }

    public Guid LeaseId { get; private set; }
    public Guid RentChargeId { get; private set; }

    public Money Amount { get; private set; } = null!;

    public DateTime PaymentDate { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public string? Reference { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Payment Create(
        Guid leaseId,
        Guid rentChargeId,
        Money amount,
        PaymentMethod paymentMethod,
        string? reference)
    {
        if (amount.Amount <= 0)
        {
            throw new InvalidOperationException("Payment amount must be greater than zero.");
        }

        var payment = new Payment
        {
            LeaseId = leaseId,
            RentChargeId = rentChargeId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            Reference = reference,
            Status = PaymentStatus.Pending,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        payment.RaiseDomainEvent(new PaymentRecordedDomainEvent(payment.Id, leaseId));

        return payment;
    }

    public void Complete()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
        {
            throw new InvalidOperationException("Only pending or processing payments can be completed.");
        }

        Status = PaymentStatus.Completed;

        RaiseDomainEvent(new PaymentCompletedDomainEvent(Id, LeaseId));
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
        {
            throw new InvalidOperationException("Only pending or processing payments can fail.");
        }

        Status = PaymentStatus.Failed;

        RaiseDomainEvent(new PaymentFailedDomainEvent(Id, LeaseId));
    }
}