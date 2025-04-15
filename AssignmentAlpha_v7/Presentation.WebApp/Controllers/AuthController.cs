using System.Security.Claims;
using Business.Services;
using Data.Entities;
using Domain.DTOs;
using Domain.DTOs.Authentication;
using Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.ViewModels.Authentications;

namespace Presentation.WebApp.Controllers;

public class AuthController(IAuthService authService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager) : Controller
// public class AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    #region Local Identity - With UserService
    
        // This Local Identity uses A bunch of extra files, such as a user service, auth service, but do not inherently
        // use the UserManager or SignInManager.
    
        #region SignUp
        
            public IActionResult SignUp()
            {
                return View();
            }
                
            [HttpPost]
            public async Task<IActionResult> SignUp(SignUpViewModel model)
            {
                ViewBag.ErrorMessage = null;
                    
                if (!ModelState.IsValid)
                    return View(model);
        
                var signUpFormData = model.MapTo<SignUpFormData>();
        
                var result = await _authService.SignUpAsync(signUpFormData);
                if (result.Succeeded)
                {
                    return RedirectToAction("SignIn", "Auth");
                }
                    
                ViewBag.ErrorMessage = result.Error;
                return View(model);
            }
        
        #endregion
        
        #region Sign In
        
            public IActionResult SignIn(string returnUrl = "~/")
            {
                ViewBag.ReturnUrl = returnUrl;
                    
                return View();
            }
                
            [HttpPost]
            public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "~/")
            {
                ViewBag.ErrorMessage = null;
                ViewBag.ReturnUrl = returnUrl;
                    
                if (!ModelState.IsValid)
                    return View(model);
                    
                var signInFormData = model.MapTo<SignInFormData>();
                    
                var result = await _authService.SignInAsync(signInFormData);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                    
                ViewBag.ErrorMessage = result.Error;
                return View(model);
            }
            
            #region ChatGPT Extension

                [HttpGet]
                public async Task<IActionResult> LocalSignInPartial(string email)
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        return BadRequest("Email is required.");
                    }

                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        return NotFound("Email does not exist.");
                    }

                    var model = new SignInViewModel { Email = email };
                    return PartialView("~/Views/Shared/Partials/Authentication/_LocalSignInFormPartial.cshtml", model);
                }

            #endregion
        
        #endregion
        
        #region Sign Out
        
            public async Task<IActionResult> SignOut(string returnUrl = "~/")
            {
                await _authService.SignOutAsync();
                    
                return LocalRedirect(returnUrl);
            }
        
        #endregion
    
    #endregion
    
    #region Local Identity - Without UserService

        // This Local Identity uses UserManager and SignInManager to function correctly. 
    
        // #region SignUp
        //
        //     [HttpGet]
        //     public IActionResult SignUp(string returnUrl = "~/")
        //     {
        //         ViewBag.ReturnUrl = returnUrl;
        //         return View();
        //     }
        //
        //     [HttpPost]
        //     public async Task<IActionResult> SignUp(SignUpViewModel model, string returnUrl = "~/")
        //     {
        //         ViewBag.ReturnUrl = returnUrl;
        //         
        //         if (!ModelState.IsValid)
        //             return View(model);
        //         
        //         var user = new AppUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
        //         var identityResult = await _userManager.CreateAsync(user, model.Password);
        //         if (identityResult.Succeeded)
        //         {
        //             var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        //             if (result.Succeeded)
        //             {
        //                 return Redirect(returnUrl); 
        //             }
        //         }
        //         
        //         ModelState.AddModelError("Unable", "Unable to create user.");
        //         return View(model);
        //     }
        //
        // #endregion
        //
        // #region Sign In
        //
        //     [HttpGet]
        //     public IActionResult SignIn(string returnUrl = "~/")
        //     {
        //         ViewBag.ReturnUrl = returnUrl;
        //         return View();
        //     }
        //
        //     [HttpPost]
        //     public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "~/")
        //     {
        //         ViewBag.returnUrl = returnUrl;
        //         
        //         if (!ModelState.IsValid)
        //             return View(model);
        //         
        //         var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        //         if (result.Succeeded)
        //         {
        //             if (string.IsNullOrEmpty(returnUrl) || returnUrl == "~/")
        //                 return RedirectToAction("Index", "Home");
        //             return LocalRedirect(returnUrl);
        //         }
        //         
        //         ModelState.AddModelError("Invalid", "Invalid email or password");
        //         return View(model);
        //     }
        //     
        //     #region ChatGPT Extension
        //
        //         [HttpGet]
        //         public async Task<IActionResult> LocalSignInPartial(string email)
        //         {
        //             if (string.IsNullOrWhiteSpace(email))
        //             {
        //                 return BadRequest("Email is required.");
        //             }
        //
        //             var user = await _userManager.FindByEmailAsync(email);
        //
        //             if (user == null)
        //             {
        //                 return NotFound("Email does not exist.");
        //             }
        //
        //             var model = new SignInViewModel { Email = email };
        //             return PartialView("~/Views/Shared/_LocalSignInPartial.cshtml", model);
        //         }
        //
        //     #endregion
        //
        // #endregion
        //
        // #region Sign Out
        //
        //     [HttpGet]
        //     public new async Task<IActionResult> SignOut()
        //     {
        //         await _signInManager.SignOutAsync();
        //         return RedirectToAction("Index", "Home");
        //     }
        //
        // #endregion

    #endregion

    #region Extrernal Authentication
        
        [HttpPost]
        public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
        {
            if (string.IsNullOrEmpty(provider))
            {
                ModelState.AddModelError("", "Invalid provider.");
                return View("SignIn");
            }
            
            var redirectUrl = Url.Action("ExternalSignInCallback", "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError("", $"Error from external provider: {remoteError}");
                return View("SignIn");
            }
            
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
                return RedirectToAction("SignIn");
            
            var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                if (string.IsNullOrEmpty(returnUrl) || returnUrl == "~/")
                    return RedirectToAction("Index", "Home");
                return LocalRedirect(returnUrl);
            }
            else
            {
                string firstName = string.Empty;
                string lastName = string.Empty;
                
                try
                {
                    firstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName)!;
                    lastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname)!;
                }
                catch { }
                
                string email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)!;
                string userName = $"ext_{externalLoginInfo.LoginProvider.ToLower()}_{email}";

                var user = new AppUser { UserName = userName, Email = email, FirstName = firstName, LastName = lastName };
                
                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, externalLoginInfo);
                    
                    string defaultRole = "User";

                    if (!await _roleManager.RoleExistsAsync(defaultRole))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                    }
                    await _userManager.AddToRoleAsync(user, defaultRole);
                    
                    await _authService.ExternalSignInAsync(user);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("SignIn");
            }
                
        }

    #endregion
}