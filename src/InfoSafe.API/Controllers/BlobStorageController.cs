﻿using Azure.Storage.Blobs;
using InfoSafe.Infra.BlobStorage.Interfaces;
using InfoSafe.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Web;

namespace InfoSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BlobStorageController : ControllerBase
    {
        private readonly ILogger<BlobStorageController> _logger;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private readonly IAzFileStorage _fileStorage;

        public BlobStorageController(
            ILogger<BlobStorageController> logger,
            FileExtensionContentTypeProvider fileExtensionContentTypeProvider,
            IAzFileStorage fileStorage)
        {
            _logger = logger;
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
            _fileStorage = fileStorage;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile blob)
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(BlobStorageController));
            scopeInfo.Add("Action", nameof(Upload));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, new { blob.FileName });

            BlobResponseVM response = new();
            BlobClient client;

            var exists = await _fileStorage.CheckIfBlobExistsAsync(blob.FileName);

            if (exists)
            {
                response.Status = $"File with name {blob.FileName} already exists. Please use another name to store your file.";
                response.Error = true;
            }
            else
            {
                if (!_fileExtensionContentTypeProvider.TryGetContentType(blob.FileName, out var contentType))
                {
                    contentType = "application/actet-stream";
                }

                await using (Stream? data = blob.OpenReadStream())
                {
                    client = await _fileStorage.UploadFileAsync(data, blob.FileName, contentType);
                }

                response.Status = $"File {blob.FileName} Uploaded Successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
            }

            return Ok(response);
        }

        [HttpGet()]
        public async Task<ActionResult> ListFiles(string? prefix)
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(BlobStorageController));
            scopeInfo.Add("Action", nameof(ListFiles));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, new { prefix });

            var files = new List<BlobVM>();

            var (uri, cloudBlobs) = await _fileStorage.ListFileBlobsAsync(prefix);

            foreach (var file in cloudBlobs)
            {
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new BlobVM
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }

            return Ok(files.OrderBy(x => x.Name));
        }

        [HttpPost()]
        public async Task<ActionResult> DownloadFile([FromBody] BlobRequestVM value)
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(BlobStorageController));
            scopeInfo.Add("Action", nameof(DownloadFile));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, new { value.FileName });

            value.FileName = HttpUtility.UrlDecode(value.FileName);
            var client = await _fileStorage.GetBlobClientAsync(value.FileName);
            if (await client.ExistsAsync())
            {
                var content = await client.DownloadContentAsync();
                string contentType = content.Value.Details.ContentType;

                var streamToWrite = new MemoryStream();
                await _fileStorage.DownloadFileAsync(client, streamToWrite);

                string base64String = Convert.ToBase64String(streamToWrite.ToArray());

                return Ok(new BlobVM
                {
                    Content = $"data:{contentType};base64,{base64String}",
                    Name = client.Name,
                    ContentType = contentType
                });
            }

            return null;
        }

        [HttpDelete("{fileName}")]
        public async Task<ActionResult> DeleteFile(string fileName)
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(BlobStorageController));
            scopeInfo.Add("Action", nameof(DeleteFile));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, new { fileName });

            fileName = HttpUtility.UrlDecode(fileName);
            BlobResponseVM response = new();
            var client = await _fileStorage.GetBlobClientAsync(fileName);

            if (await client.ExistsAsync())
            {
                await _fileStorage.DeleteFileAsync(client);

                response.Error = false;
                response.Status = $"File: {fileName} has been successfully deleted.";
            }
            else
            {
                response.Error = true;
                response.Status = $"File with name {fileName} not found.";
            }

            return Ok(response);
        }
    }
}