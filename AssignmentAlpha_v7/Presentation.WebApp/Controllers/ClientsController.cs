using Business.Models;
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
public class ClientsController(IClientService clientService, ILogger<ClientsController> logger, IImageServiceHelper imageServiceHelper, IImageService imageService) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IImageService _imageService = imageService;
    private readonly IImageServiceHelper _imageServiceHelper = imageServiceHelper;
    private readonly ILogger<ClientsController> _logger = logger;

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

    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientViewModel viewModel)
    {
        _logger.LogInformation("Received AddClient request with data: {@ViewModel}", viewModel);

        // Check if the form model is valid
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            _logger.LogWarning("Validation failed for AddClient request. Errors: {@Errors}", errors);
            return BadRequest(new { success = false, errors });
        }

        // Handle image upload if any image is provided
        if (viewModel.ClientImage is { Length: > 0 })
        {
            var imageUploadResult = await HandleImageUploadAsync(viewModel.ClientImage, viewModel.ClientName);
            if (!imageUploadResult.Succeeded)
            {
                return BadRequest(new { success = false, error = imageUploadResult.Error ?? "Image upload failed." });
            }

            // Assign the uploaded image URL to the view model
            viewModel.ImageId = imageUploadResult.Result!.Last().Id;
        }

        // Map view model to form data
        var formMapped = viewModel.MapTo<AddClientFormData>();

        // Handle Address (if provided)
        if (viewModel.Address is not null)
        {
            formMapped.Address = viewModel.Address.MapTo<AddressFormData>();
        }

        // Attempt to create the client
        try
        {
            var result = await _clientService.CreateClientAsync(formMapped);

            if (result.Succeeded)
            {
                _logger.LogInformation("Client created successfully with ID: {ClientId}", result.Result);
                return Ok(new { success = true });
            }

            _logger.LogWarning("Client creation failed with error: {Error}", result.Error);
            return BadRequest(new { success = false, error = result.Error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating the client.");
            return Problem("An unexpected error occurred while creating the client.");
        }
    }

    private async Task<ImageServiceResult> HandleImageUploadAsync(IFormFile clientImage, string clientName)
    {
        try
        {
            var imageUploadResult = await _imageServiceHelper.SaveImageAsync(clientImage, "clients", new ImageFormData
            {
                AltText = $"{clientName}'s Avatar"
            });

            if (imageUploadResult.Succeeded && imageUploadResult.Result?.Any() == true)
            {
                return imageUploadResult; // Return the result if upload succeeded
            }
            else
            {
                _logger.LogWarning("Image upload failed or returned no result.");
                return new ImageServiceResult { Succeeded = false, Error = "Image upload failed." };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading the client image.");
            return new ImageServiceResult { Succeeded = false, Error = "An error occurred while uploading the image." };
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditClient(EditClientViewModel viewModel)
    {
        _logger.LogInformation("Received EditClient request with data: {@ViewModel}", viewModel);

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            _logger.LogWarning("Validation failed for EditClient request. Errors: {@Errors}", errors);
            return BadRequest(new { success = false, errors });
        }

        var clientServiceResult = await _clientService.GetClientByIdAsync(viewModel.Id);
        var client = clientServiceResult.Result?.FirstOrDefault();

        if (client == null)
        {
            _logger.LogWarning("Client with ID {ClientId} not found.", viewModel.Id);
            return NotFound(new { success = false, error = "Client not found." });
        }

        // Handle image upload if provided
        if (viewModel.ClientImage != null && viewModel.ClientImage.Length > 0)
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
                _logger.LogWarning("Image upload failed or returned no result.");
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
            var result = await _clientService.UpdateClientAsync(viewModel.Id, formMapped);

            if (result.Succeeded)
            {
                _logger.LogInformation("Client with ID {ClientId} updated successfully.", viewModel.Id);
                return Ok(new { success = true });
            }

            _logger.LogWarning("Failed to update client with ID {ClientId}. Error: {Error}", viewModel.Id, result.Error);
            return Problem("Unable to update client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the client with ID {ClientId}", viewModel.Id);
            return Problem("An error occurred while processing your request.");
        }
    }
}