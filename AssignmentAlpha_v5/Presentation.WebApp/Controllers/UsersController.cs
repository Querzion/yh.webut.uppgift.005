using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.WebApp.Controllers;

public class UsersController(AppDbContext context) : Controller
{
    private readonly AppDbContext _context = context;

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var users = await _context.Users
            .Where(x => x.FirstName.Contains(term) || x.LastName.Contains(term) || x.Email.Contains(term))
            .Select(x => new { x.Id, x.Image.ImageUrl, FullName = x.FirstName + " " + x.LastName })
            .ToListAsync();

        return Json(users);
    }
}