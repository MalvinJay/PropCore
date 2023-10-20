using PropCore.Domain.Common;

namespace PropCore.Domain.Entities;

public sealed class MaintenanceComment : Entity
{
    private MaintenanceComment()
    {
    }

    public Guid MaintenanceRequestId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public static MaintenanceComment Create(
        Guid maintenanceRequestId,
        Guid authorId,
        string content)
    {
        return new MaintenanceComment
        {
            MaintenanceRequestId = maintenanceRequestId,
            AuthorId = authorId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
    }
}