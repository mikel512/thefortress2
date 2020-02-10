using Microsoft.AspNetCore.Mvc;

namespace TheFortress.Controllers
{
    public class TrustedController : Controller
    {
        // GET
        public IActionResult HouseShows()
        {
            return View();
        }
    }
}