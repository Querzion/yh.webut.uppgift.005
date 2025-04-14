using Business.Interfaces;
using Domain.DTOs.Registrations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.ViewModels.Logins;
using Presentation.WebApp.ViewModels.Registrations;
using Presentation.WebApp.ViewModels.SignUps;

namespace Presentation.WebApp.Controllers;

#region From the "Tips & Trix - ViewModels & Models" Video

    public class AuthController(IAuthService authenticationService) : Controller
    {
        private readonly IAuthService _authenticationService = authenticationService;

        
        public IActionResult Login(string returnUrl = "~/")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                returnUrl = "~/";
        
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel form, string returnUrl = "~/")
        {
            ViewBag.ErrorMessage = "";
        
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.LoginAsync(form);
                if (result)
                    return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
            }
            
            ViewBag.ErrorMessage = "Incorrect email or password.";
            return View(form);
            
        }
        
        public IActionResult SignUp()
        {
            return View();
        }
        
        // Convert from ViewModel >> DTO
        [HttpPost]
        public async Task<IActionResult> SignUp(MemberSignUpViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            // You can do it this way or through the ViewModel.
            // var memberSignUpDto = new MemberSignUpForm()
            // {
            //     FirstName = model.FirstName,
            //     LastName = model.LastName,
            //     Email = model.Email,
            //     Password = model.Password
            // };
            
            // ViewModel mapping is chosen here.
            // MemberSignUpForm form = model;
            // await _authenticationService.SignUpAsync(form);
            
            // OR I can do it like this and just put the model in, since the DTO is being converted in the viewModel
            // IF the convertion from ViewModel to DTO is added to the ViewModel, and This is called a complicit mapping
            await _authenticationService.SignUpAsync(model);
            
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddMember(AddMemberViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            
            await _authenticationService.AddMemberAsync(model);
            
            return View();
        }
    }

#endregion

#region From the "Tips & Trix - Autentisering och Team Members" Video (https://youtu.be/PO46Yhz_ejg?si=1SHrM_Te8U_wKgPt)

    // public class AuthController(IAuthService authService) : Controller
    // {
    //     private readonly IAuthService _authService = authService;
    //     
    //     public IActionResult Login(string returnUrl = "~/")
    //     {
    //         if (string.IsNullOrWhiteSpace(returnUrl))
    //             returnUrl = "~/";
    //
    //         ViewBag.ReturnUrl = returnUrl;
    //         return View();
    //     }
    //     
    //     [HttpPost]
    //     public async Task<IActionResult> Login(MemberLoginForm form, string returnUrl = "~/")
    //     {
    //         ViewBag.ErrorMessage = "";
    //     
    //         if (ModelState.IsValid)
    //         {
    //             var result = await _authService.LoginAsync(form);
    //             if (result)
    //                 return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
    //         }
    //         
    //         ViewBag.ErrorMessage = "Incorrect email or password.";
    //         return View(form);
    //         
    //     }
    //     
    //     public IActionResult SignUp()
    //     {
    //         ViewBag.ErrorMessage = "";
    //         
    //         return View();
    //     }
    //     
    //     [HttpPost]
    //     public async Task<IActionResult> SignUp(MemberRegistrationForm form)
    //     {
    //         if (ModelState.IsValid)
    //         {
    //             var result = await _authService.SignUpAsync(form);
    //             if (result)
    //                 return LocalRedirect("~/");
    //         }
    //     
    //         ViewBag.ErrorMessage = "";
    //         return View(form);
    //     }
    //
    //     public async Task<IActionResult> Logout()
    //     {
    //         await _authService.LogoutAsync();
    //         
    //         return LocalRedirect("~/");
    //     }
    // }

#endregion

#region AuthController - From the 'Add Identity Authentication Manually' Video.

    // public class AuthController(UserService userService, SignInManager<ApplicationUser> signInManager) : Controller
    // {
    //     private readonly UserService _userService = userService;
    //     private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    //
    //     // [Route("register")]
    //     public IActionResult SignUp()
    //     {
    //         return View();
    //     }
    //     
    //     [HttpPost]
    //     public async Task<IActionResult> SignUp(UserSignUpForm form)
    //     {
    //         if (!ModelState.IsValid)
    //             return View(form);
    //
    //         if (await _userService.ExistsAsync(form.Email))
    //         {
    //             ModelState.AddModelError("Exists", "User already exists.");
    //             return View(form);
    //         }
    //         
    //         var result = await _userService.CreateAsync(form);
    //         if (result)
    //             return RedirectToAction("SignIn", "Auth");
    //         
    //         ModelState.AddModelError("Not Created", "User was not created.");
    //         return View(form);
    //     }
    //     
    //     // [Route("login")]
    //     public IActionResult SignIn(string returnUrl = "/")
    //     {
    //         ViewBag.ReturnUrl = returnUrl;
    //         ViewBag.ErrorMessage = "";
    //         return View();
    //     }
    //     
    //     [HttpPost]
    //     public async Task<IActionResult> SignIn(UserSignInForm form, string returnUrl = "/")
    //     {
    //         #region With the returnUrl (Valid)
    //
    //             if (ModelState.IsValid)
    //             {
    //                 var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
    //                 if (result.Succeeded)
    //                 {
    //                     if (Url.IsLocalUrl(returnUrl))
    //                         return Redirect(returnUrl);
    //                     
    //                     return RedirectToAction("Index", "Home");
    //                 }
    //             }
    //                 
    //             ViewBag.ReturnUrl = returnUrl;
    //             ViewBag.ErrorMessage = "Invalid email or password.";
    //             return View(form);
    //
    //         #endregion
    //         
    //         #region With the returnUrl (Not Valid)
    //
    //             // if (!ModelState.IsValid)
    //             // {
    //             //     ViewBag.ReturnUrl = returnUrl;
    //             //     ViewBag.ErrorMessage = "Invalid email or password.";
    //             //     return View(form);
    //             // }
    //             //
    //             // var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
    //             // if (result.Succeeded)
    //             // {
    //             //     if (Url.IsLocalUrl(returnUrl))
    //             //         return Redirect(returnUrl);
    //             //     
    //             //     return RedirectToAction("Index", "Home");
    //             // }
    //             //
    //             // ViewBag.ReturnUrl = returnUrl;
    //             // ViewBag.ErrorMessage = "Invalid email or password.";
    //             // return View(form);
    //
    //         #endregion
    //         
    //         #region Without the returnUrl
    //
    //             // if (ModelState.IsValid)
    //             // {
    //             //     var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
    //             //     if (result.Succeeded)
    //             //         return RedirectToAction("Index", "Home");
    //             // }
    //             //
    //             // ViewData["ErrorMessage"] = "Invalid login attempt.";
    //             // return View(form);
    //
    //         #endregion
    //     }
    //     
    //     public new async Task<IActionResult> SignOut()
    //     {
    //         await _signInManager.SignOutAsync();
    //         return RedirectToAction("Index", "Home");
    //     }
    //     
    //     // Gets status codes from the UserService.cs if it returns an int
    //     // [HttpPost]
    //     // public async Task<IActionResult> SignUp(UserSignUpForm form)
    //     // {
    //     //     if (!ModelState.IsValid)
    //     //         return View(form);
    //     //     
    //     //     var result = await _userService.CreateAsync(form);
    //     //     switch (result)
    //     //     {
    //     //         case 201:
    //     //             return RedirectToAction("SignIn", "Auth");
    //     //         
    //     //         case 400:
    //     //         {
    //     //             ModelState.AddModelError("Invalid Fields", "Required fields not valid.");
    //     //             return View(form);
    //     //         }
    //     //         
    //     //         case 409:
    //     //         {
    //     //             ModelState.AddModelError("Exists", "User already exists.");
    //     //             return View(form);
    //     //         }
    //     //         
    //     //         default:
    //     //         {
    //     //             ModelState.AddModelError("Unexpected Error", "An unexpected error occured.");
    //     //             return View(form);
    //     //         }
    //     //     }
    //     // }
    // }

#endregion

#region AuthController - ChatGPT Generated Code - This was for learning purposes (Slisk Lindqvist)

    // public class AuthController : Controller
    // {
    //     private readonly UserManager<ApplicationUser> _userManager;
    //     private readonly SignInManager<ApplicationUser> _signInManager;
    //
    //     public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    //     {
    //         _userManager = userManager;
    //         _signInManager = signInManager;
    //     }
    //
    //     [HttpGet]
    //     public IActionResult Register()
    //     {
    //         return View();
    //     }
    //
    //     [HttpPost]
    //     [ValidateAntiForgeryToken]
    //     public async Task<IActionResult> Register(RegisterViewModel model)
    //     {
    //         if (ModelState.IsValid)
    //         {
    //             var user = new ApplicationUser
    //             {
    //                 UserName = model.Email,
    //                 Email = model.Email,
    //                 Profile = new UserProfile
    //                 {
    //                     FirstName = model.FirstName,
    //                     LastName = model.LastName
    //                 }
    //             };
    //
    //             var result = await _userManager.CreateAsync(user, model.Password);
    //                 
    //             if (result.Succeeded)
    //             {
    //                 await _signInManager.SignInAsync(user, isPersistent: false);
    //                 return RedirectToAction("Index", "Home"); // Redirect after successful registration
    //             }
    //
    //             foreach (var error in result.Errors)
    //             {
    //                 ModelState.AddModelError(string.Empty, error.Description);
    //             }
    //         }
    //
    //         return View(model);
    //     }
    //     
    //     [HttpGet]
    //     public IActionResult Login()
    //     {
    //         return View();
    //     }
    //
    //     // POST: Login
    //     [HttpPost]
    //     [ValidateAntiForgeryToken]
    //     public async Task<IActionResult> Login(LoginViewModel model)
    //     {
    //         if (ModelState.IsValid)
    //         {
    //             var user = await _userManager.FindByEmailAsync(model.Email);
    //             if (user != null)
    //             {
    //                 var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
    //                 if (result.Succeeded)
    //                 {
    //                     return RedirectToAction("Index", "Home"); // Redirect to home after successful login
    //                 }
    //                 if (result.IsLockedOut)
    //                 {
    //                     // Handle lockout (if applicable)
    //                     return RedirectToAction("Lockout");
    //                 }
    //                 else
    //                 {
    //                     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //                 }
    //             }
    //             else
    //             {
    //                 ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //             }
    //         }
    //         return View(model); // Return the model to the view if validation fails
    //     }
    //     
    //     // GET: Lockout
    //     public IActionResult Lockout()
    //     {
    //         return View();
    //     }
    // }

#endregion