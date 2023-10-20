using PropCore.Domain.Common;

namespace PropCore.Domain.Entities;

public sealed class InspectionPhoto : Entity
{
    private InspectionPhoto()
    {
    }

    public Guid InspectionId { get; private set; }

    public string StorageKey { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public static InspectionPhoto Create(
        Guid inspectionId,
        string storageKey,
        string fileName,
        string contentType)
    {
        return new InspectionPhoto
        {
            InspectionId = inspectionId,
            StorageKey = storageKey,
            FileName = fileName,
            ContentType = contentType,
            CreatedAt = DateTime.UtcNow
        };
    }
}