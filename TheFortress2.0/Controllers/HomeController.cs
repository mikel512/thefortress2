using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheFortress.Models;
using DataAccessLibrary.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccessLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TheFortress.Controllers
{
    public class HomeController : FortressController<HomeController>
    {
        // Constructor
        public HomeController(ILogger<HomeController> logger, IStorageService storageService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext applicationDbContext,
            RoleManager<IdentityRole> roleManager) : base(logger, userManager, storageService, applicationDbContext,
            roleManager)
        {
        }

        #region Views

        public IActionResult Index()
        {
            List<string> imgArray = new List<string>();
            foreach (string s in Directory.GetFiles("wwwroot/img/").Select(f => Path.GetFileName(f)))
            {
                imgArray.Add(s);
            }

            ViewData["imgArray"] = imgArray;

            // using (FileStream stream = System.IO.File.OpenRead(@"C:\Users\mikel\Desktop\opeth-flyer.jpg"))
            // {
            //     var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            //     {
            //         Headers = new HeaderDictionary(),
            //         ContentType = "application/pdf"
            //     };
            //     var tst = new LocalConcert()
            //     {
            //         Artists = "TESTARTISTS",
            //         VenueName = "TESTVENUE",
            //         TimeStart = DateTime.Today,
            //         FlyerFile = file,
            //         
            //     };
            //     await AddToApprovalUser(tst);
            // }

            return View();
        }

        public IActionResult Concerts()
        {
            var concerts = _dbAccessLogic.ApprovedConcertsByMonth();
            ViewData["concertDictionary"] = concerts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [Authorize(Roles = "User, Artist, Trusted, Administrator")]
        public IActionResult AddToApproval()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        #endregion

        #region AjaxCalls

        [HttpPost]
        [Authorize(Roles = "User, Artist, Administrator")]
        public async Task<IActionResult> AddToApprovalUser(LocalConcert localConcert)
        {
            //check if file length is too long
            if (localConcert.FlyerFile.Length > 6000000)
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (File size exceeded).");
                // Log error
                return BadRequest(ModelState);
            }

            // Scan and upload file
            localConcert.FlyerUrl = await _storageService.StoreImageFile(localConcert.FlyerFile);

            // Get user id
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Add date to queue; admin must then approve
            _dbAccessLogic.CreateQueuedDate(localConcert, currentUserId);
            // _dbAccessLogic.CreateQueuedDate(localConcert, "371217ea-6458-40eb-ace7-4d5c83df2469");

            return Ok();
        }

        public IActionResult AddAdminMsgAjax(MessageModel messageModel)
        {
            messageModel.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _dbAccessLogic.AddAdminMessage(messageModel);
                return Ok();
            }

            return BadRequest();
        }

        [Authorize(Roles = "User, Artist, Trusted, Administrator")]
        public IActionResult AddComment(CommentModel commentModel)
        {
            // Form only adds EventId and Content properties, fill out the rest
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            string userName = _userManager.GetUserName(currentUser);
            
            // Add to model
            commentModel.UserId = currentUserId;
            commentModel.DateStamp = DateTime.Now;
            commentModel.UserName = userName;

            if (ModelState.IsValid)
            {
                int insertedId = _dbAccessLogic.AddComment(commentModel);

                return Json(new Dictionary<string, string>()
                {
                    ["0"] = insertedId.ToString(),
                    ["1"] = commentModel.UserName,
                    ["2"] = commentModel.EventId.ToString(),
                    ["3"] = commentModel.Content,
                    ["4"] = (commentModel.ParentCommentId == null) ? "null" : commentModel.ParentCommentId.ToString()
                });
            }
            //Something went wrong
            return BadRequest();
        }

        #endregion
    }
}