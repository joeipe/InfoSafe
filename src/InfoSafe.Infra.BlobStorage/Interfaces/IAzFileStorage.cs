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

        Task<Stream> DownloadFileAsync(BlobClient blobClient);

        //Task DeleteFileAsync(BlobClient blob);
    }
}