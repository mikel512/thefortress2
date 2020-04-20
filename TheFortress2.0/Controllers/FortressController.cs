using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Logic;
using DataAccessLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    // TODO move models to root project
    public class FortressController<T> : Controller
    {
        protected readonly ILogger<T> _logger;
        protected readonly IStorageService _storageService;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly DbAccessLogic _dbAccessLogic;
        protected readonly DataAccessService _dataAccessService;
        
        public FortressController(ILogger<T> logger, UserManager<IdentityUser> userManager, IStorageService storageService,
            ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _storageService = storageService;
            
            _dataAccessService = new DataAccessService(applicationDbContext);
            _dbAccessLogic = new DbAccessLogic(_dataAccessService);
        }
    }
}