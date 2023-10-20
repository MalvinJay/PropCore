using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.ValueObjects;

namespace PropCore.Domain.Entities;

public sealed class MaintenanceCost : Entity
{
    private MaintenanceCost()
    {
    }

    public Guid MaintenanceRequestId { get; private set; }
    public string Description { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public CostType CostType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static MaintenanceCost Create(
        Guid maintenanceRequestId,
        string description,
        Money amount,
        CostType costType)
    {
        return new MaintenanceCost
        {
            MaintenanceRequestId = maintenanceRequestId,
            Description = description,
            Amount = amount,
            CostType = costType,
            CreatedAt = DateTime.UtcNow
        };
    }
}