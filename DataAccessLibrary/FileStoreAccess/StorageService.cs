using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataAccessLibrary.FileStoreAccess
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private StorageCredentials Credentials
        {
            get => GetCredentials();
            set { }
        }

        private StorageCredentials GetCredentials()
        {
            return new StorageCredentials(_configuration["Storage:account1:Name"], _configuration["BlobKey"]);
        }

        public async Task<string> StoreImageFile(IFormFile file, bool  safe)
        {
            var filenameonly = Path.GetFileName(Path.GetRandomFileName()+".jpg");
            var url = _configuration["Storage:account1:Base"];
            var containerName = safe ? _configuration["Storage:account1:Containers:Flyers"] : @"prescan";
            
            // Get blob data
            var blobClient = new CloudBlobClient(new Uri(url), Credentials);
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // set final file destination
            var picBlob = container.GetBlockBlobReference(filenameonly);
            picBlob.Properties.ContentType = "image/jpg";
            // asynchronously upload file
            await picBlob.UploadFromStreamAsync(file.OpenReadStream());

            // the url image is saved to
            return url + "/" + containerName + "/" + filenameonly;
        }
        public async Task<List<string>> GetAllImgsFromBlob()
        {
            var list = new List<string>();
            var url = _configuration["Storage:account1:Base"];
            var containerName = _configuration["Storage:account1:Containers:Flyers"];
            
            // Get blob data
            var blobClient = new CloudBlobClient(new Uri(url), Credentials);
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            var results  = await container.ListBlobsSegmentedAsync("",true, BlobListingDetails.Metadata, null, null,
                null, null);
            foreach (var item in results.Results)
            {
                var blob = (CloudBlob) item;
                list.Add(blob.Uri.ToString());               
            }
            
            return list;
        }

        public async void AddQueueMessage(string jsonReport)
        {
            var url = _configuration["Storage:account1:Queues:scan"];
            var queueClient = new CloudQueueClient(new Uri( "https://storagethefortress.queue.core.windows.net"), Credentials);
            var queue = queueClient.GetQueueReference("scan");
            await queue.AddMessageAsync(new CloudQueueMessage(jsonReport));
        }

    }
}