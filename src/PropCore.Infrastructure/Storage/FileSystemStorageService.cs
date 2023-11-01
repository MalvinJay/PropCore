using Microsoft.Extensions.Logging;
using PropCore.Application.Abstractions.Storage;

namespace PropCore.Infrastructure.Storage;

public sealed class FileSystemStorageService(
    ILogger<FileSystemStorageService> logger,
    string rootPath) : IStorageService
{
    public async Task<string> UploadAsync(
        Stream content,
        string container,
        string path,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(rootPath, container, Path.GetDirectoryName(path) ?? string.Empty);

        Directory.CreateDirectory(directory);

        var fileName = Path.GetFileName(path);
        var fullPath = Path.Combine(directory, fileName);

        await using var stream = File.Create(fullPath);
        await content.CopyToAsync(stream, cancellationToken);

        var storageKey = Path.Combine(container, path).Replace('\\', '/');

        logger.LogInformation("Stored file to {StorageKey}", storageKey);

        return storageKey;
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}