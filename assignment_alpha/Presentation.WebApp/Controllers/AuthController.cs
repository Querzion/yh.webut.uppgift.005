using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class AuthController : Controller
{
    [Route("register")]
    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Login()
    {
        // TEMPORARY REROUTE
        return LocalRedirect("/projects");
        // return View();
    }
}