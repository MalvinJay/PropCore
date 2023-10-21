using PropCore.Domain.Common;
using PropCore.Domain.Events;

namespace PropCore.Domain.Entities;

public sealed class Document : AggregateRoot
{
    private Document()
    {
    }

    public Guid OrganizationId { get; private set; }

    public string EntityType { get; private set; } = null!;
    public Guid EntityId { get; private set; }

    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public string StorageKey { get; private set; } = null!;

    public long Size { get; private set; }

    public Guid UploadedBy { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Document Create(
        Guid organizationId,
        Guid entityId,
        string entityType,
        string fileName,
        string contentType,
        string storageKey,
        long size,
        Guid uploadedBy)
    {
        var document = new Document
        {
            OrganizationId = organizationId,
            EntityType = entityType,
            EntityId = entityId,
            FileName = fileName,
            ContentType = contentType,
            StorageKey = storageKey,
            Size = size,
            UploadedBy = uploadedBy,
            CreatedAt = DateTime.UtcNow
        };

        document.RaiseDomainEvent(new DocumentUploadedDomainEvent(document.Id, organizationId));

        return document;
    }
}