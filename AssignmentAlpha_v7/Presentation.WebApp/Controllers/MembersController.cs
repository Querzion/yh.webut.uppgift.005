using System.Security.Claims;
using Business.Services;
using Data.Contexts;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Admins")]
public class MembersController(AppDbContext context, IUserService userService, IWebHostEnvironment env) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IWebHostEnvironment _env = env;
    
    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var userServiceResult = await _userService.GetUsersAsync();
        
        var viewModel = new MembersViewModel
        {
            Title = "Team Members",
            AddMember = new AddMemberViewModel(),
            EditMember = new EditMemberViewModel(),
            Members = userServiceResult.Result
        };
        
        return View(viewModel);
    }
    
    // [HttpPost]
    // public async Task<IActionResult> AddMember(AddMemberViewModel formData)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         var errors = ModelState
    //             .Where(x => x.Value?.Errors.Count > 0)
    //             .ToDictionary(
    //                 kvp => kvp.Key,
    //                 kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
    //             );
    //
    //         return BadRequest(new { success = false, errors });
    //     }
    //
    //     var user = formData.MapTo<AddMemberFormData>();
    //     
    //     var result = await _userService.CreateUserAsync(user);
    //     if (result.Succeeded)
    //     {
    //         return Ok(new { success = true });
    //     }
    //     else
    //     {
    //         return Problem("Unable to submit data.");
    //     }
    //     
    //     return Ok(new { success = true });
    // }
    
    [HttpPost]
    public async Task<IActionResult> AddMember(AddMemberViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        // Handle file upload
        if (formData.UserImage != null && formData.UserImage.Length > 0)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "members");
            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(formData.UserImage.FileName);
            var fileName = $"[{DateTime.UtcNow:yyyy-MM-dd}].[{Guid.NewGuid()}]{extension}";
            var savePath = Path.Combine(uploadFolder, fileName);

            await using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await formData.UserImage.CopyToAsync(stream);
            }

            var imageUrl = Path.Combine("/uploads/members", fileName).Replace("\\", "/");

            // Set the image URL on the view model
            formData.ImageUrl = imageUrl;
        }

        // Optionally: Compose the DateOfBirth from dropdowns
        formData.ComposeDateOfBirth();

        var address = new UserAddressFormData
        {
            StreetName = formData.StreetName,
            PostalCode = formData.PostalCode,
            City = formData.City
        };
        
        // Map to the domain model (this part needs to ensure ImageEntity gets the URL)
        var formMapped = formData.MapTo<AddMemberFormData>();

        // Make sure ImageEntity is populated from the ImageUrl
        if (!string.IsNullOrEmpty(formData.ImageUrl))
        {
            formMapped.Image = new ImageFormData
            {
                ImageUrl = formData.ImageUrl,
                AltText = "Avatar"
            };
        }

        formMapped.Address = address;
        
        var result = await _userService.CreateUserAsync(formMapped);
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return Problem("Unable to submit data.");
    }
    
    [HttpPost]
    public async Task<IActionResult> EditMember(EditMemberViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        // Ensure the user exists
        var user = await _userService.GetUserByIdAsync(viewModel.Id);
        if (user == null)
        {
            return NotFound(new { success = false, error = "User not found." });
        }

        // Handle file upload
        if (viewModel.UserImage != null && viewModel.UserImage.Length > 0)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "members");
            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(viewModel.UserImage.FileName);
            var fileName = $"[{DateTime.UtcNow:yyyy-MM-dd}].[{Guid.NewGuid()}]{extension}";
            var savePath = Path.Combine(uploadFolder, fileName);

            await using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await viewModel.UserImage.CopyToAsync(stream);
            }

            viewModel.ImageUrl = Path.Combine("/uploads/members", fileName).Replace("\\", "/");
        }

        // Compose date of birth from dropdowns
        viewModel.ComposeDateOfBirth();

        // Map view model to domain form
        var form = new EditMemberForm
        {
            Id = viewModel.Id,
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            JobTitle = viewModel.JobTitle,
            Email = viewModel.Email,
            PhoneNumber = viewModel.PhoneNumber,
            DateOfBirth = viewModel.DateOfBirth,
            Image = !string.IsNullOrEmpty(viewModel.ImageUrl)
                ? new ImageFormData
                {
                    ImageUrl = viewModel.ImageUrl,
                    AltText = "Updated Avatar"
                }
                : null,
            Address = (!string.IsNullOrEmpty(viewModel.StreetName) ||
                       !string.IsNullOrEmpty(viewModel.PostalCode) ||
                       !string.IsNullOrEmpty(viewModel.City))
                ? new UserAddressFormData
                {
                    StreetName = viewModel.StreetName,
                    PostalCode = viewModel.PostalCode,
                    City = viewModel.City
                }
                : null
        };

        var result = await _userService.UpdateUserAsync(viewModel.Id, form);
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return Problem(result.Error ?? "Unable to update user data.");
    }
        
    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var users = await context.Users
            .Where(x => x.FirstName!.Contains(term) || x.LastName!.Contains(term) || x.Email!.Contains(term))
            .Select(x => new { x.Id, x.Image!.ImageUrl, FullName = x.FirstName + " " + x.LastName })
            .ToListAsync();

        return Json(users);
    }
    
    [AllowAnonymous]
    [Route("denied")]
    public IActionResult Denied()
    {
        return View();
    }
}