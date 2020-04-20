using System.Security.Claims;
using System.Threading.Tasks;
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
    public class TrustedController : FortressController<TrustedController>
    {
        public TrustedController(ILogger<TrustedController> logger, IStorageService storageService,
            UserManager<IdentityUser> userManager, 
            ApplicationDbContext applicationDbContext, 
            RoleManager<IdentityRole> roleManager) : base(logger, userManager, storageService,applicationDbContext, roleManager)
        {
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
                ClaimsPrincipal currentUser = this.User;
                string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                // Add date to queue; admin must then approve
                _dbAccessLogic.CreateQueuedDate(localConcert, currentUserId);
                // _dbAccessLogic.CreateQueuedDate(localConcert, "371217ea-6458-40eb-ace7-4d5c83df2469");

                return Ok();
            }

            return BadRequest();
        }
    }
}