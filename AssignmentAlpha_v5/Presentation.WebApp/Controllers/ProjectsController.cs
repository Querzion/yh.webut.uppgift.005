using System.Runtime.InteropServices.JavaScript;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Domain.DTOs.Adds;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService) : Controller
{
    private readonly IProjectService _projectService = projectService;

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var model = new ProjectsViewModel
        {
            // Projects = await _projectService.GetProjectsAsync(),
        };
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(AddProjectViewModel model)
    {
        var addProjectFormData = model.MapTo<AddProjectFormData>();
        var result = await _projectService.CreateProjectAsync(addProjectFormData);
        
        return Json(new { });
    }
    
    [HttpPost]
    public IActionResult Update(EditProjectViewModel model)
    {
        return Json(new { });
    }
    
    [HttpPost]
    public IActionResult Delete(string id)
    {
        return Json(new { });
    }
}