using DataAccessLibrary.FileStoreAccess;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
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
            // TODO use correct read function for house shows
            var shows = Read.ApprovedShowsByMonth();
            ViewData["HouseShows"] = shows;
            return View();
        }

    }
}