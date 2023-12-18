using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MessengerApiServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiServices.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private static readonly FileExtensionContentTypeProvider s_fileExtensionProvider = new ();

        public ImageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _configuration = configuration;
        }

        async Task<string> IImageService.UploadImageAsync(Guid id, IFormFile image)
        {
            using var content = image.OpenReadStream();

            var extension = Path.GetExtension(image.FileName);
            var date = DateTime.Now.ToString()
                .Replace(" ", "-")
                .Replace("/", "-")
                .Replace(":", "-");
            var blobName = $"{id}-{date}{extension}";

            var blobContainerName = _configuration.GetSection("AzureBlobStorage:ContainerName").Value!;
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(content, new BlobHttpHeaders
            {
                ContentType = GetContentType(blobName)
            });

            return blobName;
        }

        private static string GetContentType(string fileName)
        {
            if (!s_fileExtensionProvider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        private async Task UploadImageAsync(string imagePath)
        {
            var blobContainerName = _configuration.GetSection("AzureBlobStorage:ContainerName").Value!;
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            var fileName = Path.GetFileName(imagePath);
            var blobClient = containerClient.GetBlobClient(fileName);
            using var stream = File.OpenRead(imagePath);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = GetContentType(fileName)
            });
        }
    }
}
