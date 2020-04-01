using DataAccessLibrary.FileStoreAccess;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TheFortress.Controllers
{
    // TODO move models to root project
    public class FortressController<T> : Controller
    {
        protected readonly DataAccess _dataAccess;
        protected readonly ILogger<T> _logger;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IStorageService _storageService;
        protected readonly DbRead Read;
        protected readonly DbInsert Insert;
        protected readonly DbDelete Delete;
        
        public FortressController(ILogger<T> logger, UserManager<IdentityUser> userManager, IStorageService storageService,
            ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _dataAccess = new DataAccess(applicationDbContext);
            _storageService = storageService;

            Read = new DbRead(_dataAccess);
            Insert = new DbInsert(_dataAccess);
            Delete = new DbDelete(_dataAccess);
        }
    }
}