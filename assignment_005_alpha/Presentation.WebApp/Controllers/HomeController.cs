using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Home";

        return View();
    }
}