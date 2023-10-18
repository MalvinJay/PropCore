using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.ValueObjects;

namespace PropCore.Domain.Entities;

public sealed class Unit : AggregateRoot
{
    private Unit()
    {
    }

    public Guid PropertyId { get; private set; }

    public string UnitNumber { get; private set; } = null!;
    public int Bedrooms { get; private set; }
    public decimal Bathrooms { get; private set; }
    public decimal SquareFeet { get; private set; }

    public Money MonthlyRent { get; private set; } = null!;
    public Money SecurityDeposit { get; private set; } = null!;

    public UnitStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    public static Unit Create(
        Guid propertyId,
        string unitNumber,
        int bedrooms,
        decimal bathrooms,
        decimal squareFeet,
        Money monthlyRent,
        Money securityDeposit)
    {
        return new Unit
        {
            PropertyId = propertyId,
            UnitNumber = unitNumber,
            Bedrooms = bedrooms,
            Bathrooms = bathrooms,
            SquareFeet = squareFeet,
            MonthlyRent = monthlyRent,
            SecurityDeposit = securityDeposit,
            Status = UnitStatus.Available,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkOccupied()
    {
        Status = UnitStatus.Occupied;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAvailable()
    {
        Status = UnitStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkMaintenance()
    {
        Status = UnitStatus.Maintenance;
        UpdatedAt = DateTime.UtcNow;
    }
}