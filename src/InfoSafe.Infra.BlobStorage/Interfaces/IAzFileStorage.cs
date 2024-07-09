using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace InfoSafe.Infra.BlobStorage.Interfaces
{
    public interface IAzFileStorage
    {
        Task<BlobClient> UploadFileAsync(Stream fileStream, string blobName, string contentType);

        Task<bool> CheckIfBlobExistsAsync(string blobName);

        Task<(string, List<BlobItem>)> ListFileBlobsAsync(string? prefix = null);

        Task<BlobClient> GetBlobClientAsync(string blobName);

        Task DownloadFileAsync(BlobClient client, Stream targetStream);

        Task DeleteFileAsync(BlobClient client);

        Task<(string title, string description)> GetBlobMetadataAsync(BlobClient client);

        Task UpdateBlobMetadataAsync(BlobClient client, string title, string description);
    }
}