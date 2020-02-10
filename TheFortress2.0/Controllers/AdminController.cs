using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    
    public class AdminController : FortressController<AdminController>
    {
        public AdminController(ILogger<AdminController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager) : base(logger, userManager, applicationDbContext, roleManager)
        {
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["LocalConcerts"] = Read.GetLocalConcerts();
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
            return Json(new Dictionary<string,string>() { ["0"] = codeId.ToString()});
        }
        [HttpPost]
        public IActionResult AddRoleAjax(AspNetRole aspNetRole)
        {
            string roleName = aspNetRole.Name;
            if (!_roleManager.RoleExistsAsync(roleName).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = roleName;
                IdentityResult roleResult = _roleManager.
                CreateAsync(role).Result;
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult DeleteConcertAjax(int eventConcertId, int localConcertId)
        {
            Delete.DeleteConcert(eventConcertId, localConcertId);
            return Json(new Dictionary<string, string>() { ["0"] = localConcertId.ToString() });
        }
        [HttpGet]
        public IActionResult DeleteCodeAjax(int codeId)
        {
            Delete.DeleteCode(codeId);
            return Json(new Dictionary<string, string>() { ["0"] = codeId.ToString() });
        }
        [HttpGet]
        public IActionResult DeleteQueueEntryAjax(int queueId)
        {
            Delete.DeleteQueueEntry(queueId);
            return RedirectToAction("Index");
        }
        #endregion

    }
}