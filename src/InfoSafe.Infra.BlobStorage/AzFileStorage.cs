using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using InfoSafe.Infra.BlobStorage.Interfaces;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace InfoSafe.Infra.BlobStorage
{
    public class AzFileStorage : IAzFileStorage
    {
        private readonly string _storageConnectionString;

        private readonly string _containerNameInfosafe = "infosafe";
        private readonly string _containerNameInfosafeArchive = "infosafe-archive";

        private readonly string _metadataKeyTitle = "title";
        private readonly string _metadataKeyDescription = "description";

        public AzFileStorage(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public async Task<BlobClient> UploadFileAsync(Stream fileStream, string blobName, string contentType)
        {
            var container = await GetInfosafeContainerAsync();

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
            var container = await GetInfosafeContainerAsync();

            BlobClient client = container.GetBlobClient(blobName);

            return await client.ExistsAsync();
        }

        public async Task<(string, List<BlobItem>)> ListFileBlobsAsync(string? prefix = null)
        {
            var cloudBlobs = new List<BlobItem>();
            var container = await GetInfosafeContainerAsync();

            await foreach (BlobItem blob in container.GetBlobsAsync(BlobTraits.Metadata, BlobStates.None, prefix))
            {
                cloudBlobs.Add(blob);
            }

            return (container.Uri.ToString(), cloudBlobs);
        }

        public async Task<BlobClient> GetBlobClientAsync(string blobName)
        {
            var container = await GetInfosafeContainerAsync();

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

        public async Task OverwriteFileAsync(BlobClient client, Stream fileStream, string contentType)
        {
            try
            {
                BlobProperties properties = await client.GetPropertiesAsync();
                var etagCondition = new BlobRequestConditions()
                {
                    IfMatch = properties.ETag
                };

                var headers = new BlobHttpHeaders
                {
                    ContentType = contentType
                };
                await client.UploadAsync(fileStream, headers, conditions: etagCondition);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == BlobErrorCode.ConditionNotMet)
                {
                    throw new ArgumentException("Concurrency Error");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteFileAsync(BlobClient client)
        {
            try
            {
                BlobProperties properties = await client.GetPropertiesAsync();
                var etagCondition = new BlobRequestConditions()
                {
                    IfMatch = properties.ETag
                };

                await client.DeleteAsync(conditions: etagCondition);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == BlobErrorCode.ConditionNotMet)
                {
                    throw new ArgumentException("Concurrency Error");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Metadata
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
            try
            {
                BlobProperties properties = await client.GetPropertiesAsync();
                var etagCondition = new BlobRequestConditions()
                {
                    IfMatch = properties.ETag
                };

                var metadata = SetMetadata(title, description);

                await client.SetMetadataAsync(metadata, conditions: etagCondition);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == BlobErrorCode.ConditionNotMet)
                {
                    throw new ArgumentException("Concurrency Error");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // SAS
        public string GetBlobUriWithSasToken(BlobClient client)
        {
            var sasToken = client.GenerateSasUri(BlobSasPermissions.Read, DateTime.Now.AddDays(1));

            return sasToken.ToString();
        }

        //Archive
        public async Task ArchiveFileAsync(BlobClient client)
        {
            var container = await GetInfosafeArchiveContainerAsync();

            BlobClient copyClient = container.GetBlobClient(client.Name);
            BlobCopyFromUriOptions copyOptions = new()
            {
                AccessTier = AccessTier.Archive
            };
            var copyOp = await copyClient.StartCopyFromUriAsync(client.Uri, copyOptions);
            await copyOp.WaitForCompletionAsync();

            await client.DeleteIfExistsAsync();
        }

        #region Private Container

        private async Task<BlobContainerClient> GetInfosafeContainerAsync()
        {
            return await GetContainerAsync(_containerNameInfosafe);
        }

        private async Task<BlobContainerClient> GetInfosafeArchiveContainerAsync()
        {
            return await GetContainerAsync(_containerNameInfosafeArchive);
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