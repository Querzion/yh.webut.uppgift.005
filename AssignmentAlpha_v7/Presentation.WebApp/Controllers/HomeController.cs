using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Example SessionCookie
        // Response.Cookies.Append("SessionCookie", "Essential", new CookieOptions
        // {
        //     IsEssential = true,
        //     Expires = DateTimeOffset.UtcNow.AddYears(1)
        // });
        
        return View();
    }
}