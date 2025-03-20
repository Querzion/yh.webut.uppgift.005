using System.ComponentModel.DataAnnotations;
using Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Data;
using Presentation.WebApp.Models;

namespace Presentation.WebApp.Controllers;

public class TestController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TestController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserRegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create new ApplicationUser
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, "DefaultPassword123!"); // Default password or allow user to set it

            if (result.Succeeded)
            {
                // Create UserProfile
                var userProfile = new UserProfile
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Redirect after successful registration
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
}

