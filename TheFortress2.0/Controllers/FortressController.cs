using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    public class FortressController<T> : Controller
    {
        
        protected readonly ILogger<T> _logger;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected DataRead Read;
        protected DataInsert Insert;
        protected DataDelete Delete;
        
        public FortressController(ILogger<T> logger, UserManager<IdentityUser> userManager, 
            ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            
            Read = new DataRead(applicationDbContext);
            Insert = new DataInsert(applicationDbContext);
            Delete = new DataDelete(applicationDbContext);
        }
    }
}