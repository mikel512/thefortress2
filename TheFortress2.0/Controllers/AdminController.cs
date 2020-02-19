using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.FileStoreAccess;
using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : FortressController<AdminController>
    {
        public AdminController(ILogger<AdminController> logger, UserManager<IdentityUser> userManager,IStorageService storageService,
            ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager) : base(logger,
            userManager, storageService ,applicationDbContext, roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["LocalConcerts"] = Read.GetApprovedLocalConcerts();
            ViewData["HouseShows"] = Read.GetApprovedHouseShows();
            ViewData["Codes"] = Read.GetAllTrustedCodes();
            ViewData["ConcertQueue"] = Read.GetQueueDash();
            return View();
        }

        public IActionResult Concerts()
        {
            ViewData["Concerts"] = Read.GetLocalConcerts();
            return View();
        }

        public IActionResult HouseShows()
        {
            ViewData["Shows"] = Read.GetHouseShows();
            return View();
        }

        public IActionResult Users()
        {
            ViewData["Users"] = Read.GetUsersWithRoles();
            ViewData["Roles"] = Read.GetRoles();
            return View();
        }

        public IActionResult Inbox()
        {
            ViewData["Messages"] = Read.GetAdminMessages();
            return View();
        }

        #region AjaxActions

        [HttpPost]
        public IActionResult AddConcertAjax(LocalConcert localConcert)
        {
            // 0 : parent row Id(event), 1: child row id (concert)
            var dictionary = Insert.CreateConcertDate(localConcert);
            return Json(dictionary);
        }

        [HttpPost]
        public IActionResult AddCodeAjax(TrustedCode trustedCode)
        {
            int codeId = 0;
            if (!trustedCode.MaxTimesUsed.HasValue)
            {
                codeId = Insert.CreateTrustedCode(trustedCode.CodeString);
            }
            else
            {
                codeId = Insert.CreateTrustedCode(trustedCode.CodeString, trustedCode.MaxTimesUsed);
            }

            return Json(new Dictionary<string, string>() {["0"] = codeId.ToString()});
        }

        [HttpPost]
        public IActionResult AddRoleAjax(AspNetRole aspNetRole)
        {
            string roleName = aspNetRole.Name;
            if (!_roleManager.RoleExistsAsync(roleName).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = roleName;
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }

            return RedirectToAction("Users");
        }

        public IActionResult ApproveQueueItemAjax(int queueId)
        {
            if (ModelState.IsValid)
            {
                var e = _dataAccess.ExecuteProcedure("ApproveQueueItem", "",
                    Pairing.Of("@queueId", queueId));
                return Json(new Dictionary<string, string>() {["0"] = queueId.ToString()});
            }

            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult DeleteConcertAjax(int localConcertId)
        {
            Delete.DeleteConcert(localConcertId);
            return Json(new Dictionary<string, string>() {["0"] = localConcertId.ToString()});
        }
        [HttpGet]
        public IActionResult DeleteShowAjax(int showId)
        {
            Delete.DeleteConcert(showId);
            return Json(new Dictionary<string, string>() {["0"] = showId.ToString()});
        }

        [HttpGet]
        public IActionResult DeleteCodeAjax(int codeId)
        {
            Delete.DeleteCode(codeId);
            return Json(new Dictionary<string, string>() {["0"] = codeId.ToString()});
        }

        [HttpGet]
        public IActionResult DeleteQueueEntryAjax(int queueId)
        {
            Delete.DeleteQueueEntry(queueId);
            return Json(new Dictionary<string, string>() {["0"] = queueId.ToString()});
        }

        #endregion
    }
}