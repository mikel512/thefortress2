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
using DataAccessLibrary.FileStoreAccess;
using Microsoft.AspNetCore.Authorization;
using DataAccessLibrary.Security;
using Microsoft.AspNetCore.Identity;

namespace TheFortress.Controllers
{
    public class HomeController : FortressController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger, IStorageService storageService,
            UserManager<IdentityUser> userManager, 
            ApplicationDbContext applicationDbContext, 
            RoleManager<IdentityRole> roleManager) : base(logger, userManager, storageService,applicationDbContext, roleManager)
        {
        }

        public IActionResult Index()
        {
            List<string> imgArray = new List<string>();
            foreach (string s in Directory.GetFiles("wwwroot/img/").Select(f => Path.GetFileName(f)))
            {
                imgArray.Add(s);
            }

            ViewData["imgArray"] = imgArray;
            
            return View();
        }

        public IActionResult Concerts()
        {
            var concerts = Read.ApprovedConcertsByMonth();
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

        #region AjaxCalls
        public IActionResult AddAdminMsgAjax(MessageModel messageModel)
        {
            messageModel.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                Insert.AddAdminMessage(messageModel);
            }

            return Ok();
        }

        [Authorize(Roles = "User, Artist, Trusted, Administrator")]
        public IActionResult AddComment(CommentModel commentModel)
        {
            // Form only adds EventId and Content properties, fill out the rest
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            string userName = _userManager.GetUserName(currentUser);

            commentModel.UserId = currentUserId;
            commentModel.DateStamp = DateTime.Now;
            commentModel.UserName = userName;

            int insertedId = Insert.AddComment(commentModel);

            return Json(new Dictionary<string, string>()
            {
                ["0"] = insertedId.ToString(),
                ["1"] = commentModel.UserName,
                ["2"] = commentModel.EventId.ToString(),
                ["3"] = commentModel.Content,
                ["4"] = (commentModel.ParentCommentId == null)? "null" : commentModel.ParentCommentId.ToString()
            });
        }
        #endregion

    }
}