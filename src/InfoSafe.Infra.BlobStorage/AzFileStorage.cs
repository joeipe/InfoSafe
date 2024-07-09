using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using InfoSafe.Infra.BlobStorage.Interfaces;

namespace InfoSafe.Infra.BlobStorage
{
    public class AzFileStorage : IAzFileStorage
    {
        private readonly string _storageConnectionString;

        private readonly string _containerNameBaby = "baby";

        private readonly string _metadataKeyTitle = "title";
        private readonly string _metadataKeyDescription = "description";

        public AzFileStorage(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public async Task<BlobClient> UploadFileAsync(Stream fileStream, string blobName, string contentType)
        {
            var container = await GetBabyContainerAsync();

            BlobClient client = container.GetBlobClient(blobName);
            var headers = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await client.UploadAsync(fileStream, headers);

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

            await foreach (BlobItem blob in container.GetBlobsAsync(BlobTraits.Metadata, BlobStates.None, prefix))
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

        public async Task DeleteFileAsync(BlobClient client)
        {
            await client.DeleteAsync();
        }

        public async Task<(string title, string description)> GetBlobMetadataAsync(BlobClient client)
        {
            BlobProperties properties = await client.GetPropertiesAsync();

            return
                (
                    properties.Metadata.ContainsKey(_metadataKeyTitle) ? properties.Metadata[_metadataKeyTitle] : "",
                    properties.Metadata.ContainsKey(_metadataKeyDescription) ? properties.Metadata[_metadataKeyDescription] : ""
                );
        }

        public async Task UpdateBlobMetadataAsync(BlobClient client, string title, string description)
        {
            var metadata = SetMetadata(title, description);

            await client.SetMetadataAsync(metadata);
        }

        #region Private Container

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

        #endregion Private Container

        #region Private Metadata

        private IDictionary<string, string> SetMetadata(string title, string description)
        {
            IDictionary<string, string> metadata = new Dictionary<string, string>();

            metadata[_metadataKeyTitle] = title;
            metadata[_metadataKeyDescription] = description;

            return metadata;
        }

        #endregion Private Metadata
    }
}