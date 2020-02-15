using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirusTotalNet;
using VirusTotalNet.Objects;
using VirusTotalNet.ResponseCodes;
using VirusTotalNet.Results;
using Newtonsoft.Json;
using Cloudmersive.APIClient.NET.VirusScan.Api;
using Cloudmersive.APIClient.NET.VirusScan.Client;
using Cloudmersive.APIClient.NET.VirusScan.Model;
using Microsoft.AspNetCore.Http;

namespace DataAccessLibrary.Security
{
    public class VirusScanService : IVirusScanService
    {
        private readonly IConfiguration _configuration;

        public VirusScanService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> VirusTotalScan(IFormFile formfile, string fileName)
        {
            var virusTotal = new VirusTotal(_configuration["VTKey"]);

            //Use HTTPS instead of HTTP
            virusTotal.UseTLS = true;

            //Check if the file has been scanned before.
            byte[] file = ConvertToBytes(formfile);
            FileReport fileReport = await virusTotal.GetFileReportAsync(file);
            bool hasFileBeenScannedBefore = fileReport.ResponseCode == FileReportResponseCode.Present;

            var jsonReport = "";

            //If the file has been scanned before, the results are embedded inside the report.
            if (hasFileBeenScannedBefore)
            {
                jsonReport = JsonConvert.SerializeObject(fileReport);
            }
            else
            {
                // scan file and return results as json
                await virusTotal.ScanFileAsync(file, fileName);
                FileReport fileResult = await virusTotal.GetFileReportAsync(file);
                jsonReport = JsonConvert.SerializeObject(fileResult);
            }

            return jsonReport;
        }

        public bool? CloudmersiveScan(IFormFile file)
        {
            Configuration.Default.AddApiKey("Apikey", _configuration["CloudmersiveKey"]);
            var apiInstance = new ScanApi();

            VirusScanResult result = new VirusScanResult();
            try
            {
            	// Scan a file for viruses
            	result = apiInstance.ScanFile(file.OpenReadStream());
            	Debug.WriteLine(result);
            }
            catch (Exception e)
            {
            	Debug.Print("Exception when calling ScanApi.ScanFile: " + e.Message );
            }

            return result.CleanResult;
        }
        
        private byte[] ConvertToBytes(IFormFile image)
        {
            byte[] coverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            coverImageBytes = reader.ReadBytes((int)image.Length);
            return coverImageBytes;
        }
    }
}
