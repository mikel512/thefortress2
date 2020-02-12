using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    public class TrustedController : FortressController<TrustedController>
    {
        public TrustedController(ILogger<TrustedController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager) : base(logger, userManager, applicationDbContext, roleManager)
        {
        }
        // GET
        public IActionResult HouseShows()
        {
            var shows = Read.ApprovedShowsByMonth();
            ViewData["HouseShows"] = shows;
            return View();
        }

    }
}