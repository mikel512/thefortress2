﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccessLibrary.Logic;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
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
    public class ManageController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ManageController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbAccessLogic _dbAccessLogic;
        private readonly IEmailService _emailService;
        
        public ManageController(ILogger<ManageController> logger, 
            UserManager<IdentityUser> userManager, 
            ApplicationDbContext applicationDbContext, 
            IEmailService emailService,
            SignInManager<IdentityUser> signInManager)
        {
            var dataAccessService = new DataAccessService(applicationDbContext);
            _dbAccessLogic = new DbAccessLogic(dataAccessService);
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult TrustedPass()
        {
            return View();
        }
        
        [AllowAnonymous]
        public IActionResult CheckEmail()
        {
            return View();
        }
        
        #region Ajax Calls
        
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogInformation("User ID or code is null");
                return RedirectToPage("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogInformation("User not found");
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            _logger.LogInformation("User has confirmed email");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> UseTrustedPassAjax(TrustedCode trustedCode)
        {
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                int result = _dbAccessLogic.UseTrustedCode(trustedCode.CodeString.ToUpper(), currentUserId);
                if (result > 0)
                {
                    IdentityUser user = await _userManager.GetUserAsync(currentUser);
                    await _userManager.AddToRoleAsync(user, "Trusted");
                    return Ok();
                }
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
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Manage",
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Send confirmation email
                    _emailService.SendMail("admin@thefortress.me", "Admin",  email, "Confirm your email",
                    $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

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