using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Logic;
using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IStorageService _storageService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DbAccessLogic _dbAccessLogic;
        
        public AdminController(ILogger<AdminController> logger, UserManager<IdentityUser> userManager,IStorageService storageService,
            ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager
            ) 
        {
            var dataAccessService = new DataAccessService(applicationDbContext);
            _dbAccessLogic = new DbAccessLogic(dataAccessService);
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _storageService = storageService;
            
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["LocalConcerts"] = _dbAccessLogic.GetApprovedLocalConcerts();
            ViewData["HouseShows"] = _dbAccessLogic.GetApprovedHouseShows();
            ViewData["Codes"] = _dbAccessLogic.GetAllTrustedCodes();
            ViewData["ConcertQueue"] = _dbAccessLogic.GetQueueDash();
            return View();
        }

        public IActionResult Concerts()
        {
            ViewData["Concerts"] = _dbAccessLogic.GetLocalConcerts();
            return View();
        }

        public IActionResult HouseShows()
        {
            ViewData["Shows"] = _dbAccessLogic.GetHouseShows();
            return View();
        }

        public IActionResult Users()
        {
            ViewData["Users"] = _dbAccessLogic.GetUsersWithRoles();
            ViewData["Roles"] = _dbAccessLogic.GetRoles();
            return View();
        }

        public IActionResult Inbox()
        {
            ViewData["Messages"] = _dbAccessLogic.GetAdminMessages();
            return View();
        }

        #region AjaxActions

        [HttpPost]
        public async Task<IActionResult> AddConcertAjax(LocalConcert localConcert)
        {
            if (ModelState.IsValid)
            {
                localConcert.FlyerUrl = await _storageService.StoreImageFile(localConcert.FlyerFile);
                var dictionary = _dbAccessLogic.CreateConcertDate(localConcert);
                
                // 0 : parent row Id(event), 1: child row id (concert)
                return Json(dictionary);
            }

            return BadRequest();
        }

        [HttpPost]
        public IActionResult AddCodeAjax(TrustedCode trustedCode)
        {
            int codeId = 0;
            if (ModelState.IsValid)
            {
                if (!trustedCode.MaxTimesUsed.HasValue)
                {
                    codeId = _dbAccessLogic.CreateTrustedCode(trustedCode.CodeString);
                }
                else
                {
                    codeId = _dbAccessLogic.CreateTrustedCode(trustedCode.CodeString, trustedCode.MaxTimesUsed);
                }

                return Json(new Dictionary<string, string>() {["0"] = codeId.ToString()});
            }

            return BadRequest();
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

        public IActionResult ApproveQueueItemAjax(int itemId)
        {
            if (itemId > 0 && !itemId.Equals(null))
            {
                var e = _dbAccessLogic.ApproveQueueItem(itemId);
                return Json(new Dictionary<string, string>() {["0"] = itemId.ToString()});
            }

            return BadRequest();
        }

        [HttpGet]
        public IActionResult DeleteConcertAjax(int itemId)
        {
            if (itemId > 0 && !itemId.Equals(null))
            {
                _dbAccessLogic.DeleteConcert(itemId);
                return Json(new Dictionary<string, string>() {["0"] = itemId.ToString()});
            }

            return BadRequest();
        }
        [HttpGet]
        public IActionResult DeleteShowAjax(int itemId)
        {
            if (itemId > 0 && !itemId.Equals(null))
            {
                _dbAccessLogic.DeleteShow(itemId);
                return Json(new Dictionary<string, string>() {["0"] = itemId.ToString()});
            }
            
            return BadRequest();
        }

        [HttpGet]
        public IActionResult DeleteCodeAjax(int itemId)
        {
            if (itemId > 0 && !itemId.Equals(null))
            {
                _dbAccessLogic.DeleteCode(itemId);
                return Json(new Dictionary<string, string>() {["0"] = itemId.ToString()});
            }
            
            return BadRequest();
        }

        [HttpGet]
        public IActionResult DeleteQueueEntryAjax(int itemId)
        {
            if (itemId > 0 && !itemId.Equals(null))
            {
                _dbAccessLogic.DeleteQueueEntry(itemId);
                return Json(new Dictionary<string, string>() {["0"] = itemId.ToString()});
            }

            return BadRequest();
        }

        #endregion
    }
}