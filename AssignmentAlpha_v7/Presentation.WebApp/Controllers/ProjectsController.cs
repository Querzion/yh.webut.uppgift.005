using System.Runtime.InteropServices.JavaScript;
using Business.Models;
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
using Presentation.WebApp.Mappings;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.Forms;
using Presentation.WebApp.ViewModels.ListItems;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Presentation.WebApp.Controllers;

[Authorize]
[Route("admin/projects")]
public class ProjectsController(IProjectService projectService, AppDbContext context, IClientService clientService, IImageServiceHelper imageServiceHelper, IImageService imageService, ILogger<ProjectsController> logger) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly AppDbContext _context = context;
    private readonly IClientService _clientService = clientService;
    private readonly IImageService _imageService = imageService;
    private readonly IImageServiceHelper _imageServiceHelper = imageServiceHelper;
    private readonly ILogger<ProjectsController> _logger = logger;

    [Route("")]
    [HttpGet]
    // [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        // Get the projects from the service (assuming it returns a list of Project entities)
        var projectServiceResult = await _projectService.GetAllProjectsAsync();
    
        // Use the mapping extension to convert Project entities to ProjectListItemViewModel
        var projectViewModels = projectServiceResult.Result!
            .ToViewModelList();

        // Prepare the view model
        var viewModel = new ProjectsViewModel(_clientService)
        {
            Title = "Projects",
            AddProject = new AddProjectViewModel(),
            EditProject = new EditProjectViewModel(),
            Projects = projectViewModels // Use mapped list here
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

        ImageServiceResult? imageServiceResult = null;

        // Handle image upload if a file is provided
        if (model.ProjectImage != null && model.ProjectImage.Length > 0)
        {
            var metadata = new ImageFormData
            {
                AltText = "Project Image"
            };

            // Upload the image using the ImageServiceHelper
            imageServiceResult = await _imageServiceHelper.SaveImageAsync(model.ProjectImage, "projects", metadata);

            if (!imageServiceResult.Succeeded || imageServiceResult.Result == null || !imageServiceResult.Result.Any())
            {
                return Json(new { Success = false, Message = imageServiceResult.Error ?? "Image upload failed." });
            }

            // Get the first image from the result and assign its URL
            var uploadedImage = imageServiceResult.Result.First();
            model.ImageId = uploadedImage.Id;
        }

        // Map the view model to form data for project creation
        var addProjectFormData = model.MapTo<AddProjectFormData>();

        // If an image was uploaded, map the image to the project form data
        if (imageServiceResult?.Result?.Any() == true)
        {
            addProjectFormData.Image = imageServiceResult.Result
                .Select(img => new ImageFormData
                {
                    ImageUrl = img.ImageUrl,
                    AltText = img.AltText
                })
                .FirstOrDefault();
        }

        // Call the project service to create the project
        var result = await _projectService.CreateProjectAsync(addProjectFormData);

        // Handle the result of the project creation
        if (!result.Succeeded)
        {
            return Json(new { Success = false, Message = result.Error ?? "Failed to create project" });
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