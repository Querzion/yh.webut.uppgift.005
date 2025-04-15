using Business.Services;
using Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Admins")]
public class MembersController(AppDbContext context, IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;
    
    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var userServiceResult = await _userService.GetUsersAsync();
        
        var viewModel = new MembersViewModel
        {
            Title = "Team Members",
            AddMember = new AddMemberViewModel(),
            EditMember = new EditMemberViewModel(),
            Members = userServiceResult.Result
        };
        
        return View(viewModel);
    }
    
    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var users = await context.Users
            .Where(x => x.FirstName.Contains(term) || x.LastName.Contains(term) || x.Email.Contains(term))
            .Select(x => new { x.Id, x.Image.ImageUrl, FullName = x.FirstName + " " + x.LastName })
            .ToListAsync();

        return Json(users);
    }
    
    [AllowAnonymous]
    [Route("denied")]
    public IActionResult Denied()
    {
        return View();
    }
}