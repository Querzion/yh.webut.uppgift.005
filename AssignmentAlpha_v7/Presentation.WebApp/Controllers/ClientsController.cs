using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Domain.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Presentation.WebApp.Helpers;
using Presentation.WebApp.Mappings;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Managers")]
public class ClientsController(IClientService clientService, ILogger<ClientsController> logger, IImageServiceHelper imageServiceHelper, IImageService imageService, IAddressRepository addressRepository, IFileHandler fileHandler) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IAddressRepository _addressRepository = addressRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IImageServiceHelper _imageServiceHelper = imageServiceHelper;
    private readonly ILogger<ClientsController> _logger = logger;
    private readonly IFileHandler _fileHandler = fileHandler;

    [Route("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var clientServiceResult = await _clientService.GetAllClientsAsync();

        // Use the ToViewModelList extension method to map the client entities
        var clientViewModels = clientServiceResult.Result!
            .ToViewModelList(); // Map the list of clients to view models

        var viewModel = new ClientsViewModel
        {
            Title = "Clients",
            AddClient = new AddClientViewModel(),
            EditClient = new EditClientViewModel(),
            Clients = clientViewModels // Use the mapped list of view models
        };

        return View(viewModel);
    }

    #region AddClient (HttpResponse)
        
        // [HttpPost]
        // public async Task<IActionResult> AddClient(AddClientViewModel viewModel)
        // {
        //     // Validate model
        //     if (!ModelState.IsValid)
        //     {
        //         var errors = ModelState
        //             .Where(x => x.Value?.Errors.Count > 0)
        //             .ToDictionary(
        //                 kvp => kvp.Key,
        //                 kvp => new
        //                 {
        //                     Messages = kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray(),
        //                     Exception = kvp.Value?.Errors.Select(x => x.Exception?.Message).ToArray()
        //                 }
        //             );
        //
        //         return BadRequest(new { success = false, errors });
        //     }
        //
        //     // Handle image upload
        //     // if (viewModel.ClientImage is { Length: > 0 })
        //     // {
        //     //     var imageServiceResult = await _imageServiceHelper.SaveImageAsync(viewModel.ClientImage, "clients", new ImageFormData
        //     //     {
        //     //         AltText = $"{viewModel.ClientName} Logo"
        //     //     });
        //     //
        //     //     if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
        //     //     {
        //     //         var uploadedImage = imageServiceResult.Result!.Last();
        //     //         viewModel.ImageId = uploadedImage.Id;
        //     //     }
        //     //     else
        //     //     {
        //     //         ModelState.AddModelError("ClientImage", "Image upload failed.");
        //     //         return BadRequest(new { success = false, error = "Image upload failed." });
        //     //     }
        //     // }
        //     
        //     // Image Upload - Azure Blob Storage.
        //     if (viewModel.ClientImage is { Length: > 0 })
        //     {
        //         // 1. Upload the file to Azure
        //         var imageFileUri = await _fileHandler.UploadFileAsync(viewModel.ClientImage!);
        //
        //         if (string.IsNullOrEmpty(imageFileUri))
        //         {
        //             ModelState.AddModelError("ClientImage", "Image upload failed.");
        //             return BadRequest(new { success = false, error = "Image upload failed." });
        //         }
        //
        //         // 2. Create ImageFormData (metadata) instead of ImageEntity
        //         var imageFormData = new ImageFormData
        //         {
        //             ImageUrl = imageFileUri,
        //             AltText = $"{viewModel.ClientName} Logo"
        //         };
        //
        //         // 3. Save the image using ImageServiceHelper and pass ImageFormData
        //         var imageServiceResult = await _imageService.SaveImageAsync(imageFormData);
        //
        //         if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
        //         {
        //             var uploadedImage = imageServiceResult.Result!.First();  // Get the first image
        //             viewModel.ImageId = uploadedImage.Id;  // Assign the uploaded image Id to view model
        //         }
        //         else
        //         {
        //             Console.WriteLine("Image save failed. Result: " + JsonConvert.SerializeObject(imageServiceResult));
        //             ModelState.AddModelError("ClientImage", "Saving image info to database failed.");
        //             return BadRequest(new { success = false, error = "Saving image info to database failed." });
        //         }
        //     }
        //
        //     // Handle address resolution or creation
        //     if (viewModel.Address != null)
        //     {
        //         var street = CapitalizeFirstLetter(viewModel.Address.StreetName.Trim());
        //         var postal = viewModel.Address.PostalCode.Trim();
        //         var city = CapitalizeFirstLetter(viewModel.Address.City.Trim());
        //
        //         var existingAddressResult = await _addressRepository.FindEntityAsync(a =>
        //             a.StreetName == street &&
        //             a.PostalCode == postal &&
        //             a.City == city
        //         );
        //
        //         if (existingAddressResult.Succeeded && existingAddressResult.Result != null)
        //         {
        //             viewModel.AddressId = existingAddressResult.Result.Id;
        //             viewModel.Address = null;
        //         }
        //         else
        //         {
        //             var newAddress = new AddressEntity
        //             {
        //                 StreetName = street,
        //                 PostalCode = postal,
        //                 City = city
        //             };
        //
        //             var addResult = await _addressRepository.AddAsync(newAddress);
        //             if (addResult.Succeeded)
        //             {
        //                 viewModel.AddressId = newAddress.Id;
        //                 viewModel.Address = null;
        //             }
        //             else
        //             {
        //                 return BadRequest(new { success = false, error = "Failed to create address." });
        //             }
        //         }
        //     }
        //     else
        //     {
        //         viewModel.AddressId = null;
        //         viewModel.Address = null;
        //     }
        //
        //     var formData = viewModel.MapTo<AddClientFormData>();
        //
        //     var result = await _clientService.AddClientAsync(formData);
        //
        //     if (result.Succeeded)
        //     {
        //         return Ok(new { success = true });
        //     }
        //
        //     return BadRequest(new { success = false, error = result.Error ?? "Unable to submit data." });
        // }
        
    #endregion
    
    #region AddClient (JSON) - ChatGPT Rewrite
        
        [HttpPost]
        public async Task<IActionResult> AddClient(AddClientViewModel viewModel)
        {
            // Validate model
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

                return Json(new { success = false, errors });
            }

            // Handle image upload - Azure Blob Storage.
            if (viewModel.ClientImage is { Length: > 0 })
            {
                // 1. Upload the file to Azure
                var imageFileUri = await _fileHandler.UploadFileAsync(viewModel.ClientImage!);

                if (string.IsNullOrEmpty(imageFileUri))
                {
                    return Json(new { success = false, error = "Image upload failed." });
                }

                // 2. Create ImageFormData (metadata) instead of ImageEntity
                var imageFormData = new ImageFormData
                {
                    ImageUrl = imageFileUri,
                    AltText = $"{viewModel.ClientName} Logo"
                };

                // 3. Save the image using ImageServiceHelper and pass ImageFormData
                var imageServiceResult = await _imageService.SaveImageAsync(imageFormData);

                if (imageServiceResult.Succeeded && imageServiceResult.Result!.Any())
                {
                    var uploadedImage = imageServiceResult.Result!.First();  // Get the first image
                    viewModel.ImageId = uploadedImage.Id;  // Assign the uploaded image Id to view model
                }
                else
                {
                    Console.WriteLine("Image save failed. Result: " + JsonConvert.SerializeObject(imageServiceResult));
                    return Json(new { success = false, error = "Saving image info to database failed." });
                }
            }

            // Handle address resolution or creation
            if (viewModel.Address != null)
            {
                var street = CapitalizeFirstLetter(viewModel.Address.StreetName.Trim());
                var postal = viewModel.Address.PostalCode.Trim();
                var city = CapitalizeFirstLetter(viewModel.Address.City.Trim());

                var existingAddressResult = await _addressRepository.FindEntityAsync(a =>
                    a.StreetName == street &&
                    a.PostalCode == postal &&
                    a.City == city
                );

                if (existingAddressResult.Succeeded && existingAddressResult.Result != null)
                {
                    viewModel.AddressId = existingAddressResult.Result.Id;
                    viewModel.Address = null;
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
                        viewModel.AddressId = newAddress.Id;
                        viewModel.Address = null;
                    }
                    else
                    {
                        return Json(new { success = false, error = "Failed to create address." });
                    }
                }
            }
            else
            {
                viewModel.AddressId = null;
                viewModel.Address = null;
            }

            var formData = viewModel.MapTo<AddClientFormData>();

            var result = await _clientService.AddClientAsync(formData);

            if (result.Succeeded)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, error = result.Error ?? "Unable to submit data." });
        }

        
    #endregion

    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1).ToLowerInvariant();
    }

    [HttpPost]
    public async Task<IActionResult> EditClient(EditClientViewModel viewModel)
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

        var clientServiceResult = await _clientService.GetClientByIdAsync(viewModel.Id);
        var client = clientServiceResult.Result?.FirstOrDefault();

        if (client == null)
        {
            return NotFound(new { success = false, error = "Client not found." });
        }

        // Handle image upload if provided
        if (viewModel.ClientImage is { Length: > 0 })
        {
            // Delete old image if present
            if (client.Image != null && !string.IsNullOrEmpty(client.Image.ImageUrl))
            {
                await _imageService.DeleteImageAsync(client.Image.ImageUrl);
            }

            // Upload new image
            var imageUploadResult = await _imageServiceHelper.SaveImageAsync(viewModel.ClientImage, "clients", new ImageFormData
            {
                AltText = $"{viewModel.ClientName}'s Avatar"
            });

            if (imageUploadResult.Succeeded && imageUploadResult.Result?.Any() == true)
            {
                var uploadedImage = imageUploadResult.Result.First();
                viewModel.ImageId = uploadedImage.Id;
            }
            else
            {
                return BadRequest(new { success = false, error = "Image upload failed." });
            }
        }

        // Map ViewModel to FormData
        var formMapped = viewModel.MapTo<EditClientFormData>();

        // Address mapping
        formMapped.Address = viewModel.Address?.MapTo<AddressFormData>();

        // Image mapping
        if (client.Image != null)
        {
            formMapped.Image = new ImageFormData
            {
                ImageUrl = client.Image.ImageUrl, // Use Image.ImageUrl here
                AltText = $"{viewModel.ClientName}'s Avatar"
            };
        }

        try
        {
            // Update client in the service
            var result = await _clientService.UpdateClientAsync(viewModel.Id, formMapped);

            if (result.Succeeded)
            {
                return Ok(new { success = true });
            }

            return Problem("Unable to update client.");
        }
        catch (Exception)
        {
            return Problem("An error occurred while processing your request.");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClient(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ClientServiceResult
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Client ID is required."
            });
        }

        var serviceResult = await _clientService.DeleteClientAsync(id);

        if (!serviceResult.Succeeded)
        {
            if (serviceResult.StatusCode == 404)
            {
                return NotFound(serviceResult);
            }

            return StatusCode(serviceResult.StatusCode, serviceResult);
        }

        return RedirectToAction("Index");
    }
}