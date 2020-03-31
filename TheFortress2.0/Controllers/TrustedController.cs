using DataAccessLibrary.FileStoreAccess;
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
            ViewData["HouseShows"] = Read.ApprovedShowsByMonth();
            return View();
        }

    }
}