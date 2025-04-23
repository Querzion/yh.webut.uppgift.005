using System.Globalization;
using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.WebApp.Helpers;
using Presentation.WebApp.Mappings;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Admins")]
public class MembersController(IUserService userService, IImageServiceHelper imageServiceHelper, AppDbContext context, IAddressRepository addressRepository, IImageService imageService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IAddressRepository _addressRepository = addressRepository;
    private readonly IImageServiceHelper _imageServiceHelper = imageServiceHelper;
    private readonly IImageService _imageService = imageService;
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
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        Messages = kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray(),
                        Exception = kvp.Value?.Errors.Select(x => x.Exception?.Message).ToArray()
                    }
                );

            Console.WriteLine("ModelState Errors: " + JsonConvert.SerializeObject(errors));
            return BadRequest(new { success = false, errors });
        }

        // Handle image upload
        if (formData.UserImage is { Length: > 0 })
        {
            var imageServiceResult = await _imageServiceHelper.SaveImageAsync(formData.UserImage, "members", new ImageFormData
            {
                AltText = $"{formData.FirstName} {formData.LastName}'s Avatar"
            });

            if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
            {
                var uploadedImage = imageServiceResult.Result!.Last();
                formData.ImageId = uploadedImage.Id;
            }
            else
            {
                Console.WriteLine("Image upload failed. Result: " + JsonConvert.SerializeObject(imageServiceResult));
                ModelState.AddModelError("UserImage", "Image upload failed.");
                return BadRequest(new { success = false, error = "Image upload failed." });
            }
        }

        // Handle address creation or retrieval
        if (formData.Address != null)
        {
            var street = CapitalizeFirstLetter(formData.Address.StreetName.Trim());
            var postal = formData.Address.PostalCode.Trim();
            var city   = CapitalizeFirstLetter(formData.Address.City.Trim());

            var existingAddressResult = await _addressRepository.FindEntityAsync(a =>
                a.StreetName == street &&
                a.PostalCode == postal &&
                a.City == city
            );

            if (existingAddressResult.Succeeded && existingAddressResult.Result != null)
            {
                formData.AddressId = existingAddressResult.Result.Id;
                formData.Address = null; // ✅ Prevents the factory from creating a new AddressEntity
            }
            else
            {
                var newAddress = new AddressEntity
                {
                    StreetName = street,
                    PostalCode = postal,
                    City = city
                };

                var addResult = await _addressRepository.AddAsync(newAddress);
                if (addResult.Succeeded)
                {
                    formData.AddressId = newAddress.Id;
                    formData.Address = null; // ✅ Prevents the factory from creating another one
                }
                else
                {
                    return BadRequest(new { success = false, error = "Failed to create address." });
                }
            }
        }
        else
        {
            formData.AddressId = null;
            formData.Address = null; // ✅ Clear it just in case
        }
        

        // Compose DateOfBirth from the dropdown values
        formData.ComposeDateOfBirth();

        // Map the view model to the domain model for processing
        var formMapped = formData.MapTo<AddMemberFormData>();

        // Create the user via the UserService
        var result = await _userService.AddUserAsync(formMapped);

        Console.WriteLine("User creation result: " + JsonConvert.SerializeObject(result));

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return BadRequest(new { success = false, error = result.Error ?? "Unable to submit data." });
    }

    
    

    #region Old AddMember

        // [HttpPost]
        // public async Task<IActionResult> AddMember(AddMemberViewModel formData)
        // {
        //     // Check if the form model is valid
        //     if (!ModelState.IsValid)
        //     {
        //         // Log and capture detailed validation errors
        //         var errors = ModelState
        //             .Where(x => x.Value?.Errors.Count > 0)
        //             .ToDictionary(
        //                 kvp => kvp.Key, // Key is the model property
        //                 kvp => new 
        //                 {
        //                     // Capture error messages along with the exception details if available
        //                     Messages = kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray(),
        //                     Exception = kvp.Value?.Errors.Select(x => x.Exception?.Message).ToArray()
        //                 }
        //             );
        //
        //         // Log the errors for debugging purposes
        //         Console.WriteLine("ModelState Errors: " + JsonConvert.SerializeObject(errors));
        //
        //         // Return detailed errors to the client
        //         return BadRequest(new { success = false, errors });
        //     }
        //
        //     // Handle image upload using the helper
        //     if (formData.UserImage is { Length: > 0 })
        //     {
        //         // Upload image and get the ImageServiceResult
        //         var imageServiceResult = await _imageServiceHelper.SaveImageAsync(formData.UserImage, "members", new ImageFormData
        //         {
        //             // Set default AltText or use formData's AltText if you have one
        //             AltText = $"{formData.FirstName} {formData.LastName}'s Avatar"
        //         });
        //
        //         // Check if the image upload succeeded and assign the image ID
        //         if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
        //         {
        //             var uploadedImage = imageServiceResult.Result!.Last();
        //             formData.ImageId = uploadedImage.Id;
        //         }
        //         else
        //         {
        //             // Log the image upload failure for debugging
        //             Console.WriteLine("Image upload failed. Result: " + JsonConvert.SerializeObject(imageServiceResult));
        //
        //             // Handle the error case (e.g., log or return an error response)
        //             ModelState.AddModelError("UserImage", "Image upload failed.");
        //             return BadRequest(new { success = false, error = "Image upload failed." });
        //         }
        //     }
        //
        //     // Compose the DateOfBirth from the dropdown values in the view model
        //     formData.ComposeDateOfBirth();
        //
        //     // Map the view model to the domain model for processing
        //     var formMapped = formData.MapTo<AddMemberFormData>();
        //
        //     // Handle address creation or retrieval
        //     if (formData.Address != null)
        //     {
        //         var addressServiceResult = await _addressService.GetOrCreateAddressAsync(new AddressFormData
        //         {
        //             StreetName = formData.Address.StreetName,
        //             PostalCode = formData.Address.PostalCode,
        //             City = formData.Address.City
        //         });
        //
        //         if (!addressServiceResult.Succeeded)
        //         {
        //             // Log the address service result for debugging
        //             Console.WriteLine("Address service failed. Result: " + JsonConvert.SerializeObject(addressServiceResult));
        //
        //             // Handle the case where address creation or retrieval failed
        //             ModelState.AddModelError("Address", addressServiceResult.Error ?? "Address creation failed.");
        //             return BadRequest(new { success = false, error = addressServiceResult.Error });
        //         }
        //
        //         // If the address was found or created successfully, assign it to the formMapped object
        //         formMapped.AddressId = addressServiceResult.Result?.FirstOrDefault()?.Id;
        //     }
        //     else
        //     {
        //         // Optionally handle the case where no address is provided
        //         formMapped.AddressId = null; // or handle accordingly if you don't want to allow null addresses
        //     }
        //
        //     // No need to manually set the image here as it was already handled in the ImageServiceHelper
        //
        //     // Create the user via the UserService
        //     var result = await _userService.AddUserAsync(formMapped);
        //
        //     // Log the result of user creation attempt
        //     Console.WriteLine("User creation result: " + JsonConvert.SerializeObject(result));
        //
        //     // Check if the user creation was successful
        //     if (result.Succeeded)
        //     {
        //         return Ok(new { success = true });
        //     }
        //
        //     // Return a more detailed error if user creation fails
        //     return BadRequest(new { success = false, error = result.Error ?? "Unable to submit data." });
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

    #endregion
    
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
                    kvp => new
                    {
                        Messages = kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray(),
                        Exception = kvp.Value?.Errors.Select(x => x.Exception?.Message).ToArray()
                    }
                );

            Console.WriteLine("ModelState Errors: " + JsonConvert.SerializeObject(errors));
            return BadRequest(new { success = false, errors });
        }

        // Retrieve the user by ID
        var userResult = await _userService.GetUserByIdAsync(viewModel.Id);
        if (!userResult.Succeeded || userResult.Result == null)
        {
            return NotFound(new { success = false, error = "User not found.", statusCode = 404 });
        }

        // Handle image upload (if provided)
        if (viewModel.UserImage is { Length: > 0 })
        {
            // First, upload the new image
            var imageServiceResult = await _imageServiceHelper.SaveImageAsync(viewModel.UserImage, "members", new ImageFormData
            {
                AltText = $"{viewModel.FirstName} {viewModel.LastName}'s Avatar"
            });

            // Check if the upload was successful
            if (!imageServiceResult.Succeeded || imageServiceResult.Result?.Any() != true)
            {
                return BadRequest(new { success = false, error = imageServiceResult.Error ?? "Image upload failed." });
            }

            // Now that the new image is successfully uploaded, delete the old image (if any)
            // if (!string.IsNullOrEmpty(userResult.Result.ImageId))
            // {
            //     var deleteImageResult = await _imageService.DeleteImageAsync(userResult.Result.ImageId);
            //     if (!deleteImageResult.Succeeded)
            //     {
            //         return BadRequest(new { success = false, error = deleteImageResult.Error ?? "Failed to delete old image." });
            //     }
            // }

            // Assign the new image ID to the viewModel
            viewModel.ImageId = imageServiceResult.Result.Last().Id;
        }

        // Handle address creation or retrieval (Directly in the controller, no AddressService)
        string? addressId = null;
        if (viewModel.Address != null)
        {
            var existingAddressResult = await _addressRepository.GetEntityAsync(a =>
                a.StreetName!.Trim().ToLower() == viewModel.Address.StreetName.Trim().ToLower() &&
                a.PostalCode!.Trim().ToLower() == viewModel.Address.PostalCode.Trim().ToLower() &&
                a.City!.Trim().ToLower() == viewModel.Address.City.Trim().ToLower()
            );

            if (existingAddressResult.Succeeded && existingAddressResult.Result != null)
            {
                addressId = existingAddressResult.Result.Id;
            }
            else
            {
                var newAddress = new AddressEntity
                {
                    Id = Guid.NewGuid().ToString(),  // Ensure this is being set
                    StreetName = CapitalizeFirstLetter(viewModel.Address.StreetName),
                    PostalCode = viewModel.Address.PostalCode,
                    City = CapitalizeFirstLetter(viewModel.Address.City)
                };

                var addAddressResult = await _addressRepository.AddAsync(newAddress);
                await _addressRepository.SaveChangesAsync();

                if (addAddressResult.Succeeded)
                {
                    addressId = newAddress.Id;
                }
                else
                {
                    ModelState.AddModelError("Address", "Failed to add address.");
                    return BadRequest(new { success = false, error = "Address creation failed.", statusCode = 400 });
                }
            }
        }

        // Compose DateOfBirth from parts
        viewModel.ComposeDateOfBirth();

        // Map the view model to the domain model for updating the user
        var form = new EditMemberFormData
        {
            Id = viewModel.Id,
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            JobTitle = viewModel.JobTitle,
            Email = viewModel.Email,
            PhoneNumber = viewModel.PhoneNumber,
            DateOfBirth = viewModel.DateOfBirth,
            AddressId = addressId
        };

        // Call the update method to save changes
        var updateResult = await _userService.UpdateUserAsync(viewModel.Id, form);
        if (updateResult.Succeeded)
        {
            return Ok(new { success = true, statusCode = 200 });
        }

        return BadRequest(new { success = false, error = updateResult.Error ?? "Unable to update user data.", statusCode = 400 });
    }

    #region Delete Member - ChatGPT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Member ID is required.");

            var result = await _userService.DeleteUserAsync(id);

            if (!result.Succeeded)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return RedirectToAction("Index");
        }

    #endregion
    
        
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
    
    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input; // Return as is if null or empty

        // Capitalize the first letter and make the rest lowercase
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}