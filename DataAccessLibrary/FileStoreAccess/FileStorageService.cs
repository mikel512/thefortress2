using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataAccessLibrary.FileStoreAccess
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IConfiguration _configuration;

        public FileStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StorageCredentials Credentials
        {
            get => GetCredentials();
            set { }
        }

        private StorageCredentials GetCredentials()
        {
            return new StorageCredentials(_configuration["Storage:account1:Name"], _configuration["BlobKey"]);
        }

        public async Task<string> StorePrescanImage(string filename, byte[] img)
        {
            var filenameonly = Path.GetFileName(filename);
            var url = string.Concat(_configuration["Storage:account1:Containers:prescan"], filenameonly);
            
            var blob = new CloudBlockBlob(new Uri(url), Credentials);
            bool shouldUpload = true;
            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                if (blob.Properties.Length == img.Length)
                {
                    shouldUpload = false;
                }
            }
            
            if (shouldUpload) await blob.UploadFromByteArrayAsync(img, 0, img.Length);

            // the url image is saved to
            return url;
        }
        
        public byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)image.Length);
            return CoverImageBytes;
        }

    }
}