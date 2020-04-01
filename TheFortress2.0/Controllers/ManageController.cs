using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccessLibrary.FileStoreAccess;
using DataAccessLibrary.Models;
using DataAccessLibrary.Security;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheFortress.Models;

namespace TheFortress.Controllers
{
    [Authorize(Roles = "User, Artist, Trusted, Administrator")]
    public class ManageController : FortressController<ManageController>
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private IEmailService _emailService;
        public ManageController(ILogger<ManageController> logger, IStorageService storageService,
            UserManager<IdentityUser> userManager, 
            ApplicationDbContext applicationDbContext, 
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            SignInManager<IdentityUser> signInManager) : base(logger, userManager, storageService,applicationDbContext, roleManager)
        {
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult TrustedPass()
        {
            return View();
        }
        public IActionResult CheckEmail()
        {
            return View();
        }

        #region Ajax Calls

        public async Task<IActionResult> UseTrustedPassAjax(TrustedCode trustedCode)
        {
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            int result = Read.UseTrustedCode(trustedCode.CodeString.ToUpper(), currentUserId);
            if (result > 0)
            {
                IdentityUser user = await _userManager.GetUserAsync(currentUser);
                await _userManager.AddToRoleAsync(user, "Trusted");
                return Ok();
            }

            // If we get here something went wrong, return error
            return BadRequest(ModelState);
        }

        #endregion

        #region Identity Logins & Registration 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAjax(InputModel input)
        {
            //string ReturnUrl;
            //IList<AuthenticationScheme> ExternalLogins;
            //returnUrl = returnUrl ?? Url.Content("~/");
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                string email = input.RegisterModel.Email;
                var user = new IdentityUser { UserName = input.RegisterModel.UserNameReg, Email = email };
                var result = await _userManager.CreateAsync(user, input.RegisterModel.PasswordReg);
                if (result.Succeeded)
                {
                    // Add user to role
                    _userManager.AddToRoleAsync(user, (input.RegisterModel.IsArtist) ? "Artist" : "User").Wait();

                    _logger.LogInformation("User created a new account with password.");

                    // Make the confirmation code
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Send confirmation email
                    _emailService.SendMail("admin@thefortress.me", "Admin",  email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Ok();
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                        //return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAjax(InputModel input)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(input.LoginModel.UserNameLogin, input.LoginModel.PasswordLogin, input.LoginModel.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return Ok();
                }
                if (result.RequiresTwoFactor)
                {
                    //return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return BadRequest(ModelState);
                }
            }

            // If we got this far, something failed, redisplay form
        return BadRequest(ModelState);
        }
        
        #endregion

    }
}