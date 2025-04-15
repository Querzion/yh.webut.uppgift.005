using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Managers")]
public class ClientsController : Controller
{
    [Route("admin/clients")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Home";

        return View();
    }
}