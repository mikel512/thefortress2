using System.Threading.Tasks;

namespace DataAccessLibrary.FileStoreAccess
{
    public interface IFileStorageService
    {
        Task<string> StorePrescanImage(string filename, byte[] image);
    }
}