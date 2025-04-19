using Business.Services;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Helpers;
using Presentation.WebApp.Mappings;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Managers")]
public class ClientsController(IClientService clientService, IImageUploadHelper imageUploadHelper, ILogger<ClientsController> logger) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IImageUploadHelper _imageUploadHelper = imageUploadHelper;
    private readonly ILogger<ClientsController> _logger = logger;

    [Route("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var clientServiceResult = await _clientService.GetClientsAsync();

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
    
    // [HttpPost]
    // public async Task<IActionResult> AddClient(AddClientViewModel viewModel)
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
    //     // Handle file upload
    //     if (viewModel.ClientImage != null && viewModel.ClientImage.Length > 0)
    //     {
    //         var imageUrl = await _imageUploadHelper.UploadImageAsync(viewModel.ClientImage, "clients");
    //
    //         if (!string.IsNullOrEmpty(imageUrl))
    //         {
    //             viewModel.ImageUrl = imageUrl;
    //         }
    //     }
    //
    //     // Map the ViewModel to the FormData and include Address
    //     var formMapped = viewModel.MapTo<AddClientFormData>();
    //
    //     // Map Address correctly
    //     if (viewModel.Address != null)
    //     {
    //         formMapped.Address = new AddressFormData
    //         {
    //             StreetName = viewModel.Address.StreetName,
    //             PostalCode = viewModel.Address.PostalCode,
    //             City = viewModel.Address.City
    //         };
    //     }
    //
    //     if (!string.IsNullOrEmpty(viewModel.ImageUrl))
    //     {
    //         formMapped.Image = new ImageFormData
    //         {
    //             ImageUrl = viewModel.ImageUrl,
    //             AltText = "Avatar"
    //         };
    //     }
    //
    //     var result = await _clientService.CreateClientAsync(formMapped);
    //     if (result.Succeeded)
    //     {
    //         return Ok(new { success = true });
    //     }
    //
    //     return Problem("Unable to submit data.");
    // }


    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientViewModel viewModel)
    {
        _logger.LogInformation("Received AddClient request with data: {@ViewModel}", viewModel);

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            _logger.LogWarning("Validation failed for AddClient request. Errors: {@Errors}", errors);
            return BadRequest(new { success = false, errors });
        }

        // Handle file upload
        if (viewModel.ClientImage != null && viewModel.ClientImage.Length > 0)
        {
            try
            {
                var imageUrl = await _imageUploadHelper.UploadImageAsync(viewModel.ClientImage, "clients");

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    viewModel.ImageUrl = imageUrl;
                }
                else
                {
                    _logger.LogWarning("Image upload returned an empty URL for client image.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the client image.");
                return Problem("An error occurred while uploading the image.");
            }
        }

        // Map ViewModel to FormData
        var formMapped = viewModel.MapTo<AddClientFormData>();

        if (viewModel.Address != null)
        {
            formMapped.Address = new AddressFormData
            {
                StreetName = viewModel.Address.StreetName,
                PostalCode = viewModel.Address.PostalCode,
                City = viewModel.Address.City
            };
        }

        if (!string.IsNullOrEmpty(viewModel.ImageUrl))
        {
            formMapped.Image = new ImageFormData
            {
                ImageUrl = viewModel.ImageUrl,
                AltText = "Avatar"
            };
        }

        try
        {
            var result = await _clientService.CreateClientAsync(formMapped);

            if (result.Succeeded)
            {
                _logger.LogInformation("Client created successfully with ID: {@ClientId}", result.Result);
                return Ok(new { success = true });
            }
            else
            {
                // Log the single error returned in the result
                _logger.LogWarning("Client creation failed. Error: {@Error}", result.Error);
                return Problem("Unable to submit data.");
            }
        }
        catch (Exception ex)
        {
            // Log exception details
            _logger.LogError(ex, "An error occurred while creating the client.");
            return Problem("An error occurred while processing your request.");
        }
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

        var client = await _clientService.GetClientByIdAsync(viewModel.Id);
        if (client == null)
        {
            return NotFound(new { success = false, error = "Client not found." });
        }

        // Handle file upload
        if (viewModel.ClientImage != null && viewModel.ClientImage.Length > 0)
        {
            var imageUrl = await _imageUploadHelper.UploadImageAsync(viewModel.ClientImage, "clients");

            if (!string.IsNullOrEmpty(imageUrl))
            {
                viewModel.ImageUrl = imageUrl;
            }
        }

        // Map the ViewModel to the FormData and include Address
        var formMapped = viewModel.MapTo<EditClientFormData>();

        // Map Address correctly
        if (viewModel.Address != null)
        {
            formMapped.Address = new AddressFormData
            {
                StreetName = viewModel.Address.StreetName,
                PostalCode = viewModel.Address.PostalCode,
                City = viewModel.Address.City
            };
        }

        // Ensure image entity is set if an image was uploaded
        if (!string.IsNullOrEmpty(viewModel.ImageUrl))
        {
            formMapped.Image = new ImageFormData
            {
                ImageUrl = viewModel.ImageUrl,
                AltText = "Updated Avatar"
            };
        }

        var result = await _clientService.UpdateClientAsync(viewModel.Id, formMapped);
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return Problem("Unable to update client.");
    }

}