using PropCore.Domain.Common;
using PropCore.Domain.Enums;

namespace PropCore.Domain.Entities;

public sealed class InspectionItem : Entity
{
    private InspectionItem()
    {
    }

    public Guid InspectionId { get; private set; }

    public string Category { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public InspectionCondition Condition { get; private set; }

    public string? Notes { get; private set; }

    public static InspectionItem Create(
        Guid inspectionId,
        string category,
        string description,
        InspectionCondition condition,
        string? notes = null)
    {
        return new InspectionItem
        {
            InspectionId = inspectionId,
            Category = category,
            Description = description,
            Condition = condition,
            Notes = notes
        };
    }
}