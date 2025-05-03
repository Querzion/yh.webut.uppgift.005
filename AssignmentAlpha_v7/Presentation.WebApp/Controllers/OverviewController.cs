using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class OverviewController : Controller
{
    [Route("admin/overview")]
    public IActionResult Index()
    {
        Response.Cookies.Append("SessionCookie", "Essential", new CookieOptions
        {
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddYears(1)
        });
        
        return View();
    }
}