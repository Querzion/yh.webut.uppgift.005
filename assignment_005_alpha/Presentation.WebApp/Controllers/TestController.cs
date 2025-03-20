using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class TestController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Home";

        return View();
    }
}