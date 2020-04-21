using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLibrary.Logic;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    [Authorize(Roles = "Trusted, Administrator")]
    public class TrustedController : Controller
    {
        private readonly DbAccessLogic _dbAccessLogic;
        private readonly IStorageService _storageService;

        public TrustedController(IStorageService storageService, ApplicationDbContext applicationDbContext)
        {
            var dataAccessService = new DataAccessService(applicationDbContext);
            _dbAccessLogic = new DbAccessLogic(dataAccessService);
            _storageService = storageService;
        }

        // GET
        public IActionResult HouseShows()
        {
            ViewData["HouseShows"] = _dbAccessLogic.ApprovedShowsByMonth();
            return View();
        }

        public async Task<IActionResult> AddToApprovalTrusted(LocalConcert localConcert)
        {
            //check if file length is too long
            if (localConcert.FlyerFile.Length > 6000000)
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (File size exceeded).");
                // Log error
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                // Scan and upload file
                localConcert.FlyerUrl = await _storageService.StoreImageFile(localConcert.FlyerFile);
                // Get user id
                ClaimsPrincipal currentUser = User;
                string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                // Add date to queue; admin must then approve
                _dbAccessLogic.CreateQueuedDate(localConcert, currentUserId);
                // _dbAccessLogic.CreateQueuedDate(localConcert, "371217ea-6458-40eb-ace7-4d5c83df2469");

                return Ok();
            }

            return BadRequest();
        }

        public async Task<IActionResult> AddToApprovalTrusted(HouseShow houseShow)
        {
            //check if file length is too long
            if (houseShow.FlyerFile.Length > 6000000)
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (File size exceeded).");
                // Log error
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                // Scan and upload file
                houseShow.FlyerUrl = await _storageService.StoreImageFile(houseShow.FlyerFile);
                // Get user id
                ClaimsPrincipal currentUser = User;
                string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                // Add date to queue; admin must then approve
                _dbAccessLogic.CreateQueuedDate(houseShow, currentUserId);
                // _dbAccessLogic.CreateQueuedDate(localConcert, "371217ea-6458-40eb-ace7-4d5c83df2469");

                return Ok();
            }

            return BadRequest();
        }

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
            ClaimsPrincipal currentUser = User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Add date to queue; admin must then approve
            _dbAccessLogic.CreateQueuedDate(localConcert, currentUserId);
            // _dbAccessLogic.CreateQueuedDate(localConcert, "371217ea-6458-40eb-ace7-4d5c83df2469");

            return Ok();
        }
    }
}