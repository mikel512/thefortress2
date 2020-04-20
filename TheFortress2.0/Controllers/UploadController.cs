using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
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

namespace TheFortress.Controllers
{
    [Authorize(Roles = "Trusted, Administrator")]
    public class UploadController : FortressController<UploadController>
    {
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = {".jpg", ".jpeg"};
        private readonly string _targetFilePath;

        public UploadController(ILogger<UploadController> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext applicationDbContext,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            IStorageService storageService
            ) : base(logger, userManager, storageService,applicationDbContext, roleManager)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
        }

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        [HttpPost]
        [Route("Upload/UploadShowAjax")]
        public async Task<IActionResult> AddShowDateToQueue()
        {
            var postedFile = Request.Form.Files[0]; // Now you have the file in the postedFile variable.
            var houseShow = new HouseShow()
            {
                Artists = Request.Form["artists"],
                TimeStart = Convert.ToDateTime(Request.Form["timeStart"]),
                TimeEnd = (Request.Form["timeEnd"] == "")
                                          ? DateTime.MinValue
                                          : Convert.ToDateTime(Request.Form["timeEnd"]),
                VenueName = Request.Form["venue"]
            };

            //check if file length is too long
            if (postedFile.Length > _fileSizeLimit)
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (File size exceeded).");
                // Log error
                return BadRequest(ModelState);
            }

            // Upload file
            houseShow.FlyerUrl = await _storageService.StoreImageFile(postedFile);
            
            // Add the rest of the entries to database if the file upload is successful
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            _dbAccessLogic.CreateQueuedDate(houseShow, currentUserId);

            return Ok();
        }

        [HttpPost]
        [Route("Upload/UploadConcertAjax")]
        [Authorize(Roles = "User, Artist, Trusted, Administrator")]
        public async Task<IActionResult> AddConcertDateToQueue()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var postedFile = Request.Form.Files[0]; // Now you have the file in the postedFile variable.
            string artists = Request.Form["artists"];
            string venue = Request.Form["venue"];
            DateTime dateStart = Convert.ToDateTime(Request.Form["timeStart"]);
            DateTime? dateEnd = (Request.Form["timeEnd"] == "")
                ? DateTime.MinValue
                : Convert.ToDateTime(Request.Form["timeEnd"]);

            //check if file length is too long
            if (postedFile.Length > _fileSizeLimit)
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (File size exceeded).");
                // Log error
                return BadRequest(ModelState);
            }


            // Add the rest of the entries to database if the file upload is successful
            var concert = new LocalConcert()
            {
                Artists = artists,
                FlyerUrl = await _storageService.StoreImageFile(postedFile),
                TimeStart = dateStart,
                TimeEnd = dateEnd,
                VenueName = venue
            };

            // Get user id
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            // _dbAccessLogic to db
            _dbAccessLogic.CreateQueuedDate(concert, currentUserId);

            return Ok();

        }
    }
}