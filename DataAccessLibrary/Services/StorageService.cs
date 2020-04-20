using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataAccessLibrary.Services
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        private VirusScanService _scan;

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _scan = new VirusScanService(configuration);
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

        public async Task<string> StoreImageFile(IFormFile file)
        {
            // first, scan file
            var isClean = _scan.CloudmersiveScan(file) ?? false;
            
            var filenameonly = Path.GetFileName(Path.GetRandomFileName()+".jpg");
            var url = _configuration["Storage:account1:Base"];
            var containerName = isClean ? _configuration["Storage:account1:Containers:Flyers"] : @"prescan";
            
            // Get blob data
            var blobClient = new CloudBlobClient(new Uri(url), Credentials);
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // set final file destination
            var picBlob = container.GetBlockBlobReference(filenameonly);
            picBlob.Properties.ContentType = "image/jpg";
            // asynchronously upload file
            await picBlob.UploadFromStreamAsync(file.OpenReadStream());
            
            // Scan file with VT, add report in json format to message queue
            var report = await _scan.VirusTotalScan(file, file.FileName);
            AddQueueMessage(report);
            
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

        private async void AddQueueMessage(string jsonReport)
        {
            var url = _configuration["Storage:account1:Queues:scan"];
            var queueClient = new CloudQueueClient(new Uri( "https://storagethefortress.queue.core.windows.net"), Credentials);
            var queue = queueClient.GetQueueReference("scan");
            await queue.AddMessageAsync(new CloudQueueMessage(jsonReport));
        }

    }
}