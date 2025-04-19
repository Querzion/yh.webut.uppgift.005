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
public class MembersController(IUserService userService, IImageServiceHelper imageServiceHelper, AppDbContext context, IAddressService addressService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IAddressService _addressService = addressService;
    private readonly IImageServiceHelper _imageServiceHelper = imageServiceHelper;
    // private readonly IImageService _imageService;
    private readonly AppDbContext _context = context;
    // private readonly ILogger<MembersController> _logger;
    
    
    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var userServiceResult = await _userService.GetAllUsersAsync();
    
        // Use the mapping extension to convert User entities to MemberListItemViewModels
        var memberViewModels = userServiceResult.Result!
            .ToViewModelList();
    
        var viewModel = new MembersViewModel
        {
            Title = "Team Members",
            AddMember = new AddMemberViewModel(),
            EditMember = new EditMemberViewModel(),
            Members = memberViewModels
        };
    
        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMember(AddMemberViewModel formData)
    {
        // Check if the form model is valid
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
        if (formData.UserImage is { Length: > 0 })
        {
            // Upload image and get the ImageServiceResult
            var imageServiceResult = await _imageServiceHelper.SaveImageAsync(formData.UserImage, "members", new ImageFormData
            {
                // Set default AltText or use formData's AltText if you have one
                AltText = $"{formData.FirstName} {formData.LastName}'s Avatar"
            });

            // Check if the image upload succeeded and assign the image URL
            if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
            {
                // Get the URL of the first image in the collection
                formData.ImageUrl = imageServiceResult.Result!.Last().ImageUrl;
            }
            else
            {
                // Handle the error case (e.g., log or return an error response)
                ModelState.AddModelError("UserImage", "Image upload failed.");
                return BadRequest(new { success = false, error = "Image upload failed." });
            }
        }

        // Compose the DateOfBirth from the dropdown values in the view model
        formData.ComposeDateOfBirth();

        // Map the view model to the domain model for processing
        var formMapped = formData.MapTo<AddMemberFormData>();

        // Set the address for the new member using the AddressService
        var addressServiceResult = await _addressService.GetOrCreateAddressAsync(new AddressFormData
        {
            StreetName = formData.Address!.StreetName,
            PostalCode = formData.Address.PostalCode,
            City = formData.Address.City,
            County = formData.Address.County,
            Country = formData.Address.Country
        });

        if (!addressServiceResult.Succeeded)
        {
            // Handle the case where address creation or retrieval failed
            ModelState.AddModelError("Address", addressServiceResult.Error ?? "Address creation failed.");
            return BadRequest(new { success = false, error = addressServiceResult.Error });
        }

        // If the address was found or created successfully, assign it to the formMapped object
        formMapped.AddressId = addressServiceResult.Result?.FirstOrDefault()?.Id; // Assuming Result contains a list of addresses

        // No need to manually set the image here as it was already handled in the ImageServiceHelper

        // Create the user via the UserService
        var result = await _userService.CreateUserAsync(formMapped);

        // Check if the user creation was successful
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        // Return an error if the user creation fails
        return Problem("Unable to submit data.");
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
    
    // [HttpPost]
    // public async Task<IActionResult> AddMember(AddMemberViewModel formData)
    // {
    //     using (var transaction = await _context.Database.BeginTransactionAsync())
    //     {
    //         try
    //         {
    //             // Ensure ModelState is valid
    //             if (!ModelState.IsValid)
    //             {
    //                 var errors = ModelState
    //                     .Where(x => x.Value?.Errors.Count > 0)
    //                     .ToDictionary(
    //                         kvp => kvp.Key,
    //                         kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
    //                     );
    //
    //                 return BadRequest(new { success = false, errors });
    //             }
    //
    //             // Handle image upload
    //             if (formData.UserImage != null && formData.UserImage.Length > 0)
    //             {
    //                 var imageUrl = await _imageUploadHelper.UploadImageAsync(formData.UserImage, "members");
    //
    //                 if (!string.IsNullOrEmpty(imageUrl))
    //                 {
    //                     formData.ImageUrl = imageUrl;
    //                 }
    //             }
    //
    //             formData.ComposeDateOfBirth();
    //
    //             // Map to domain model
    //             var formMapped = formData.MapTo<AddMemberFormData>();
    //
    //             // Set address and image entities
    //             formMapped.Address = new AddressFormData
    //             {
    //                 StreetName = formData.Address.StreetName,
    //                 PostalCode = formData.Address.PostalCode,
    //                 City = formData.Address.City
    //             };
    //
    //             if (!string.IsNullOrEmpty(formData.ImageUrl))
    //             {
    //                 formMapped.Image = new ImageFormData
    //                 {
    //                     ImageUrl = formData.ImageUrl,
    //                     AltText = "Avatar"
    //                 };
    //             }
    //
    //             // Create user in a single transaction
    //             var result = await _userService.CreateUserAsync(formMapped);
    //
    //             if (!result.Succeeded)
    //             {
    //                 _logger.LogError("User creation failed: {Error}", result.Error);
    //                 return StatusCode(500, new { success = false, error = "User creation failed." });
    //             }
    //
    //             // Commit the transaction if all is good
    //             await transaction.CommitAsync();
    //             return Ok(new { success = true });
    //         }
    //         catch (Exception ex)
    //         {
    //             // Rollback the transaction on error
    //             await transaction.RollbackAsync();
    //             _logger.LogError(ex, "Error while processing AddMember request.");
    //             return StatusCode(500, new { success = false, error = "An unexpected error occurred while processing the request." });
    //         }
    //     }
    // }


    
    [HttpPost]
    public async Task<IActionResult> EditMember(EditMemberViewModel viewModel)
    {
        // Check if the form model is valid
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
        if (user == null!)
        {
            return NotFound(new { success = false, error = "User not found." });
        }

        // Handle image upload with metadata (ImageFormData)
        if (viewModel.UserImage is { Length: > 0 })
        {
            // Create the ImageFormData metadata
            var metadata = new ImageFormData
            {
                AltText = $"{viewModel.FirstName} {viewModel.LastName}'s Avatar" // Set default alt text
            };

            // Upload image
            var imageServiceResult = await _imageServiceHelper.SaveImageAsync(viewModel.UserImage, "members", metadata);
            
            if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
            {
                viewModel.ImageUrl = imageServiceResult.Result!.FirstOrDefault()?.ImageUrl;
            }
            else
            {
                ModelState.AddModelError("UserImage", "Image upload failed.");
                return BadRequest(new { success = false, error = "Image upload failed." });
            }
        }

        // Compose DateOfBirth from parts
        viewModel.ComposeDateOfBirth();

        // Map the view model to the form data for updating
        var form = new EditMemberFormData
        {
            Id = viewModel.Id,
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            JobTitle = viewModel.JobTitle,
            Email = viewModel.Email,
            PhoneNumber = viewModel.PhoneNumber,
            DateOfBirth = viewModel.DateOfBirth
        };

        // Assign image data if available (after upload)
        if (!string.IsNullOrEmpty(viewModel.ImageUrl))
        {
            // Check if the AltText is the default format or not
            var altText = string.IsNullOrEmpty(viewModel.Image?.AltText) || 
                          viewModel.Image.AltText != $"{viewModel.FirstName} {viewModel.LastName}'s Avatar"
                          ? $"{viewModel.FirstName} {viewModel.LastName}'s Avatar"
                          : viewModel.Image.AltText;

            form.Image = new ImageFormData
            {
                ImageUrl = viewModel.ImageUrl,
                AltText = altText
            };
        }

        // Handle Address: only if provided, resolve AddressId via service
        if (viewModel.Address != null &&
            (!string.IsNullOrEmpty(viewModel.Address.StreetName) ||
             !string.IsNullOrEmpty(viewModel.Address.PostalCode) ||
             !string.IsNullOrEmpty(viewModel.Address.City)))
        {
            var addressServiceResult = await _addressService.GetOrCreateAddressAsync(new AddressFormData
            {
                StreetName = viewModel.Address.StreetName,
                PostalCode = viewModel.Address.PostalCode,
                City = viewModel.Address.City,
                County = viewModel.Address.County,
                Country = viewModel.Address.Country
            });

            if (!addressServiceResult.Succeeded)
            {
                ModelState.AddModelError("Address", addressServiceResult.Error ?? "Address processing failed.");
                return BadRequest(new { success = false, error = addressServiceResult.Error });
            }

            form.AddressId = addressServiceResult.Result?.FirstOrDefault()?.Id;
        }

        // Call the update method to save changes
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

        var users = await _context.Users
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