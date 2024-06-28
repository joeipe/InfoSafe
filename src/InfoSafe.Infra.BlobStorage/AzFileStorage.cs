using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using InfoSafe.Infra.BlobStorage.Interfaces;

namespace InfoSafe.Infra.BlobStorage
{
    public class AzFileStorage : IAzFileStorage
    {
        private readonly string _containerNameBaby = "baby";

        private readonly string _storageConnectionString;

        public AzFileStorage(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }
        public async Task<BlobClient> UploadFileAsync(Stream fileStream, string blobName, string contentType)
        {
            var container = await GetBabyContainerAsync();

            BlobClient client = container.GetBlobClient(blobName);

            await client.UploadAsync(fileStream, new BlobHttpHeaders
            {
                ContentType = contentType
            });

            return client;
        }

        public async Task<bool> CheckIfBlobExistsAsync(string blobName)
        {
            var container = await GetBabyContainerAsync();

            BlobClient client = container.GetBlobClient(blobName);

            return await client.ExistsAsync();
        }

        public async Task<(string, List<BlobItem>)> ListFileBlobsAsync(string? prefix = null)
        {
            var cloudBlobs = new List<BlobItem>();
            var container = await GetBabyContainerAsync();

            await foreach (BlobItem blob in container.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix))
            {
                cloudBlobs.Add(blob);
            }

            return (container.Uri.ToString(), cloudBlobs);
        }

        public async Task<BlobClient> GetBlobClientAsync(string blobName)
        {
            var container = await GetBabyContainerAsync();

            BlobClient client = container.GetBlobClient(blobName);

            return client;
        }

        public async Task DownloadFileAsync(BlobClient client, Stream targetStream)
        {
            if (await client.ExistsAsync())
            {
                await client.DownloadToAsync(targetStream);
            }
        }

        /*
        public async Task DeleteFileAsync(BlobClient blob)
        {
            if (blob == null)
                throw new ArgumentNullException(nameof(blob));

            await blob.DeleteAsync();
        }*/

        #region Private

        private async Task<BlobContainerClient> GetBabyContainerAsync()
        {
            return await GetContainerAsync(_containerNameBaby);
        }

        private async Task<BlobContainerClient> GetContainerAsync(string containerName)
        {
            var container = new BlobContainerClient(_storageConnectionString, containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null);
            return container;
        }

        #endregion Private
    }
}