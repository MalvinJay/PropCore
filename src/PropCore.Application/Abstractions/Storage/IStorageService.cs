namespace PropCore.Application.Abstractions.Storage;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string container, string path, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}