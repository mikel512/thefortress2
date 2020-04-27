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
using DataAccessLibrary.Logic;
using Microsoft.AspNetCore.Authorization;
using DataAccessLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TheFortress.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDbAccessLogic _dbAccessLogic;

        // Constructor
        public HomeController(IDbAccessLogic dbAccessLogic)
        {
            _dbAccessLogic = dbAccessLogic;
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

            return View();
        }

        public IActionResult Concerts()
        {
            var concerts = _dbAccessLogic.ApprovedConcertsByMonth();
            // ViewData["concertDictionary"] = concerts;

            return View(concerts);
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
        public IActionResult MessageAdmin(MessageModel messageModel)
        {
            messageModel.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _dbAccessLogic.AddAdminMessage(messageModel);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize(Roles = "User, Artist, Trusted, Administrator")]
        public IActionResult AddComment(CommentModel commentModel)
        {
            // Form only adds EventId and Content properties, fill out the rest
            commentModel.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            commentModel.UserName = User.FindFirst(ClaimTypes.Name).Value;
            commentModel.DateStamp = DateTime.Now;

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