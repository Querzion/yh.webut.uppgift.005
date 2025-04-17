using System.Runtime.InteropServices.JavaScript;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.WebApp.Helpers;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.Forms;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Presentation.WebApp.Controllers;

[Authorize]
[Route("admin/projects")]
public class ProjectsController(IProjectService projectService, AppDbContext context, IClientService clientService, IImageUploadHelper imageUploadHelper) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly AppDbContext _context = context;
    private readonly IClientService _clientService = clientService;
    private readonly IImageUploadHelper _imageUploadHelper = imageUploadHelper;

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var projectServiceResult = await _projectService.GetProjectsAsync();
        
        var viewModel = new ProjectsViewModel(_clientService)
        {
            Title = "Projects",
            AddProject = new AddProjectViewModel(),
            EditProject = new EditProjectViewModel(),
            Projects = projectServiceResult.Result!
        };
        
        return View(viewModel);
    }
    
    [Route("{id}")]
    public async Task<IActionResult> Index(string id)
    {
        var project = await _context.Projects
            .Include(x => x.ProjectMembers)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (project == null)
            return NotFound();

        var viewModel = new ProjectsViewModel(_clientService);
        await viewModel.PopulateClientOptionsAsync();

        viewModel.SelectedProject = project;

        return View(viewModel);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Project ID is required.");

        var project = await _context.Projects
            .Include(x => x.ProjectMembers)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (project == null)
            return NotFound();

        var viewModel = new ProjectsViewModel(_clientService);
        await viewModel.PopulateClientOptionsAsync();
        viewModel.SelectedProject = project;

        return View("Index", viewModel); // or return View("Details", viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(ProjectEntity model, string selectedUserIds)
    {
        // if (!ModelState.IsValid)
        //     return View("Index", model);

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
    
    // POST: AddProject
    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { Success = false, Message = "Model is invalid" });
        }

        // Handle image upload using the helper
        if (model.ProjectImage != null && model.ProjectImage.Length > 0)
        {
            var imageUrl = await _imageUploadHelper.UploadImageAsync(model.ProjectImage, "projects");

            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }
        }

        var addProjectFormData = model.MapTo<AddProjectFormData>();

        // Set image entity if we got an image URL
        if (!string.IsNullOrEmpty(model.ImageUrl))
        {
            addProjectFormData.Image = new ImageFormData
            {
                ImageUrl = model.ImageUrl,
                AltText = "Project Image"
            };
        }

        var result = await _projectService.CreateProjectAsync(addProjectFormData);

        if (!result.Succeeded)
        {
            return Json(new { Success = false, Message = result.Error });
        }

        return Json(new { Success = true });
    }


    // POST: EditProject
    [HttpPost]
    public async Task<IActionResult> EditProject(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { Success = false, Message = "Model is invalid" });
        }

        // Map ViewModel to Service Model
        var editProjectFormData = model.MapTo<EditProjectFormData>(); // Ensure your MapTo works here

        // Call the service to update the project
        var result = await _projectService.UpdateProjectAsync(editProjectFormData);

        if (!result.Succeeded)
        {
            return Json(new { Success = false, Message = result.Error });
        }

        return Json(new { Success = true });
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
    
    // [HttpPost]
    // public async Task<IActionResult> Tag(ProjectsViewModel model, string selectedUserIds)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return View("Index", model);
    //     }
    //
    //     var result = await _projectService.TagProjectAsync(model, selectedUserIds);
    //
    //     if (result.Succeeded)
    //     {
    //         // Redirect to the index or another page after successful tagging
    //         return RedirectToAction("Index");
    //     }
    //     else
    //     {
    //         // Return to the same view with error message if tagging failed
    //         ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred.");
    //         return View("Index", model);
    //     }
    // }
    
    private async Task PopulateClientOptionsAsync(ProjectFormViewModel viewModel)
    {
        var result = await _clientService.GetClientsAsync();
        if (!result.Succeeded) return;

        var clients = result.Result ?? new List<Client>();

        viewModel.ClientOptions = clients.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.ClientName
        }).ToList();
    }
}