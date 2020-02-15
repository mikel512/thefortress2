using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataAccessLibrary.FileStoreAccess
{
    public interface IStorageService
    {
        Task<string> StoreFile(IFormFile image, bool safe);
        void AddQueueMessage(string jsonReport);
    }
}