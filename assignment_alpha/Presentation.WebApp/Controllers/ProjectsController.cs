using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
    [Route("")]
    public IActionResult Projects()
    {
        return View();
    }
}