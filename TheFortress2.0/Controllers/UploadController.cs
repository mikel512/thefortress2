using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using TheFortress.Data;
using TheFortress.Filters;
using TheFortress.Utilities;

namespace TheFortress.Controllers
{
    public class UploadController : FortressController<UploadController>
    {
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg" };
        private readonly string _targetFilePath;
        public UploadController(ILogger<UploadController> logger, 
            UserManager<IdentityUser> userManager, 
            ApplicationDbContext applicationDbContext, 
            RoleManager<IdentityRole> roleManager,
            IConfiguration config) : base(logger, userManager, applicationDbContext, roleManager)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            
            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
        }

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        // public UploadController(ILogger<UploadController> logger, IConfiguration config)
        // {
        //     _logger = logger;
        //     _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        //
        //     // To save physical files to a path provided by configuration:
        //     _targetFilePath = config.GetValue<string>("StoredFilesPath");
        //
        //     // To save physical files to the temporary files folder, use:
        //     //_targetFilePath = Path.GetTempPath();
        // }

        [HttpPost]
        [Route("Upload/UploadAjax")]
        [Authorize(Roles = "User, Artist, Trusted, Administrator")]
        public async Task<IActionResult> AddConcertDateToQueue()
        {
            var postedFile = Request.Form.Files[0];
            // Now you have the file in the postedFile variable.

            string artists = Request.Form["artists"];
            string venue = Request.Form["venue"];
            DateTime dateStart = Convert.ToDateTime(Request.Form["timeStart"]);
            DateTime? dateEnd = (Request.Form["timeEnd"] == "")? DateTime.MinValue : Convert.ToDateTime(Request.Form["timeEnd"]);
                

            //check if file length is too long
            if (postedFile.Length > _fileSizeLimit)
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (File size exceeded).");
                // Log error
                return BadRequest(ModelState);
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                    postedFile.FileName);
            var trustedFileNameForFileStorage = Path.GetRandomFileName() + ".jpg";

            using (var targetStream = System.IO.File.Create(
                Path.Combine(_targetFilePath, trustedFileNameForFileStorage)))
            {
                await postedFile.CopyToAsync(targetStream);

                _logger.LogInformation(
                    "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                    "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                    trustedFileNameForDisplay, _targetFilePath,
                    trustedFileNameForFileStorage);
            }
            // Add the rest of the entries to database if the file upload is successful
            var concert = new LocalConcert()
            {
                Artists = artists,
                FlyerUrl = Path.Combine(_targetFilePath, trustedFileNameForFileStorage),
                TimeStart = dateStart,
                TimeEnd = dateEnd,
                VenueName = venue
            };
            //ClaimsPrincipal currentUser = this.User;
            //string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //insert.CreateQueuedDate(concert, currentUserId);

            return Ok();
        }
        #region snippet_UploadPhysical
        [HttpPost]
        [DisableFormValueModelBinding]
        //[EnableBuffering]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhysical()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }
            if (Request.ContentLength == 0)
                return BadRequest();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File",
                            $"The request couldn't be processed (Error 2).");
                        // Log error

                        return BadRequest(ModelState);
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        using (var targetStream = System.IO.File.Create(
                            Path.Combine(_targetFilePath, trustedFileNameForFileStorage)))
                        {
                            await targetStream.WriteAsync(streamedFileContent);

                            _logger.LogInformation(
                                "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                trustedFileNameForDisplay, _targetFilePath,
                                trustedFileNameForFileStorage);
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return Created(nameof(UploadController), null);
        }
        #endregion

    }
}
