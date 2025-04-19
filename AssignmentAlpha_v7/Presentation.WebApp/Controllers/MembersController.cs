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
using Presentation.WebApp.Helpers;
using Presentation.WebApp.Mappings;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Admins")]
public class MembersController(AppDbContext context, IUserService userService, IImageUploadHelper imageUploadHelper, ILogger<MembersController> logger) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IImageUploadHelper _imageUploadHelper = imageUploadHelper;
    private readonly ILogger<MembersController> _logger = logger;
    
    
    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var userServiceResult = await _userService.GetUsersAsync();
    
        // Use the mapping extension to convert User entities to MemberListItemViewModels
        var memberViewModels = userServiceResult.Result!
            .ToViewModelList();
    
        var viewModel = new MembersViewModel
        {
            Title = "Team Members",
            AddMember = new AddMemberViewModel(),
            EditMember = new EditMemberViewModel(),
            Members = memberViewModels // Use mapped list here
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
    //     // Handle image upload using the helper
    //     if (formData.UserImage != null && formData.UserImage.Length > 0)
    //     {
    //         var imageUrl = await _imageUploadHelper.UploadImageAsync(formData.UserImage, "members");
    //
    //         if (!string.IsNullOrEmpty(imageUrl))
    //         {
    //             formData.ImageUrl = imageUrl;
    //         }
    //     }
    //
    //     // Compose the DateOfBirth from dropdowns
    //     formData.ComposeDateOfBirth();
    //
    //     // Map to domain model
    //     var formMapped = formData.MapTo<AddMemberFormData>();
    //
    //     // Set address
    //     formMapped.Address = new AddressFormData
    //     {
    //         StreetName = formData.Address.StreetName,
    //         PostalCode = formData.Address.PostalCode,
    //         City = formData.Address.City
    //     };
    //
    //     // Set image entity if uploaded
    //     if (!string.IsNullOrEmpty(formData.ImageUrl))
    //     {
    //         formMapped.Image = new ImageFormData
    //         {
    //             ImageUrl = formData.ImageUrl,
    //             AltText = "Avatar"
    //         };
    //     }
    //
    //     var result = await _userService.CreateUserAsync(formMapped);
    //
    //     if (result.Succeeded)
    //     {
    //         return Ok(new { success = true });
    //     }
    //
    //     return Problem("Unable to submit data.");
    // }
    
    [HttpPost]
    public async Task<IActionResult> AddMember(AddMemberViewModel formData)
    {
        try
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

            // Handle image upload using the helper
            if (formData.UserImage != null && formData.UserImage.Length > 0)
            {
                var imageUrl = await _imageUploadHelper.UploadImageAsync(formData.UserImage, "members");

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    formData.ImageUrl = imageUrl;
                }
            }

            // Compose the DateOfBirth from dropdowns
            formData.ComposeDateOfBirth();

            // Map to domain model
            var formMapped = formData.MapTo<AddMemberFormData>();

            // Set address
            formMapped.Address = new AddressFormData
            {
                StreetName = formData.Address.StreetName,
                PostalCode = formData.Address.PostalCode,
                City = formData.Address.City
            };

            // Set image entity if uploaded
            if (!string.IsNullOrEmpty(formData.ImageUrl))
            {
                formMapped.Image = new ImageFormData
                {
                    ImageUrl = formData.ImageUrl,
                    AltText = "Avatar"
                };
            }

            var result = await _userService.CreateUserAsync(formMapped);

            if (result.Succeeded)
            {
                return Ok(new { success = true });
            }

            return Problem("Unable to submit data.");
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error while processing AddMember request");

            // Return a 500 with more detailed error information
            return StatusCode(500, new { success = false, error = ex.Message });
        }
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

        var user = await _userService.GetUserByIdAsync(viewModel.Id);
        if (user == null)
        {
            return NotFound(new { success = false, error = "User not found." });
        }

        if (viewModel.UserImage != null && viewModel.UserImage.Length > 0)
        {
            var imageUrl = await _imageUploadHelper.UploadImageAsync(viewModel.UserImage, "members");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                viewModel.ImageUrl = imageUrl;
            }
        }

        viewModel.ComposeDateOfBirth();

        var form = new EditMemberFormData
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
            Address = (!string.IsNullOrEmpty(viewModel.Address!.StreetName) ||
                       !string.IsNullOrEmpty(viewModel.Address!.PostalCode) ||
                       !string.IsNullOrEmpty(viewModel.Address!.City))
                ? new AddressFormData
                {
                    StreetName = viewModel.Address!.StreetName,
                    PostalCode = viewModel.Address!.PostalCode,
                    City = viewModel.Address!.City
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