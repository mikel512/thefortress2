using System.IO;
using System.Net;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using DataAccessLibrary.Security;

namespace TheFortress.Utilities
{
    public class UploadFile
    {
        public string FilePath { get; set; }

        public async Task Upload(IFormFile file, string path)
        {
            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                file.FileName);
            var trustedFileNameForFileStorage = Path.GetRandomFileName() + ".jpg";

            var byteArr = ConvertToBytes(file);
            var scan = new VirusScan();
            scan.TestScan(byteArr);
            
            var storagePath = Path.Combine(path, trustedFileNameForFileStorage);
            await using (var targetStream = System.IO.File.Create(storagePath))
            {
                // this line writes the image
                await file.CopyToAsync(targetStream);

                // _logger.LogInformation(
                //     "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                //     "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                //     trustedFileNameForDisplay, _targetFilePath,
                //     trustedFileNameForFileStorage);
            }

            FilePath = path;
        }
        private byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)image.Length);
            return CoverImageBytes;
        }
        
        
    }
}