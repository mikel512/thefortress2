using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataAccessLibrary.Security
{
    public interface IVirusScanService
    {
        Task<string> VirusTotalScan(IFormFile formfile, string fileName);
        bool? CloudmersiveScan(IFormFile file);
    }
}