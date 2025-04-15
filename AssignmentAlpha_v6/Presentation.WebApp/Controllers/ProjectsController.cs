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
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService, AppDbContext context) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly AppDbContext _context = context;

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var project = await _context.Projects
            .Include(x => x.ProjectMembers)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == "1");
        
        return View(project);
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
    
    [HttpPost]
    public async Task<IActionResult> Tag(ProjectEntity model, string selectedUserIds)
    {
        if (!ModelState.IsValid)
            return View("Index",model);

        var existingMembers = await _context.ProjectMembers
            .Where(m => m.ProjectId == model.Id)
            .ToListAsync();
        
        _context.ProjectMembers.RemoveRange(existingMembers);

        if (!string.IsNullOrEmpty(selectedUserIds))
        {
            var userIds = JsonSerializer.Deserialize<List<int>>(selectedUserIds);
            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    _context.ProjectMembers.Add(new ProjectMemberEntity
                    {
                        ProjectId = model.Id, 
                        UserId = userId.ToString()
                    });
                }
            }
        }

        _context.Update(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}