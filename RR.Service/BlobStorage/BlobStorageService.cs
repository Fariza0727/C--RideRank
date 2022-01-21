using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using RR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly AppSettings _appsettings;

        public BlobStorageService(IOptions<AppSettings> appsettings)
        {
            _appsettings = appsettings.Value;
            
        }
        public async Task<string> UploadAvatarAsync(IFormCollection form, string fileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_appsettings.AzureBlobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_appsettings.AzureBlobContainerName);
            containerClient.CreateIfNotExists();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            var res = await blobClient.UploadAsync(form.Files[0].OpenReadStream());
            return await Task.FromResult(blobClient.Uri.AbsoluteUri);
            
        }
        public void Dispose()
        {
            
        }
    }
}
