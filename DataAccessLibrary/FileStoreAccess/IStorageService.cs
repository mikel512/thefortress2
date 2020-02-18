using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataAccessLibrary.FileStoreAccess
{
    public interface IStorageService
    {
        Task<string> StoreImageFile(IFormFile image, bool safe);
        Task<List<string>> GetAllImgsFromBlob();
        void AddQueueMessage(string jsonReport);
    }
}