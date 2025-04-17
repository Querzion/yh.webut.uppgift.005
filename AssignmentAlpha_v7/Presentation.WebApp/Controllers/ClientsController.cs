using Business.Services;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Helpers;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.Controllers;

[Authorize(Policy = "Managers")]
public class ClientsController(IClientService clientService, IImageUploadHelper imageUploadHelper) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IImageUploadHelper _imageUploadHelper = imageUploadHelper;

    [Route("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var clientServiceResult = await _clientService.GetClientsAsync();
        
        var viewModel = new ClientsViewModel
        {
            Title = "Clients",
            AddClient = new AddClientViewModel(),
            EditClient = new EditClientViewModel(),
            Clients = clientServiceResult.Result!
        };
        
        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientViewModel viewModel)
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
        if (viewModel.ClientImage != null && viewModel.ClientImage.Length > 0)
        {
            var imageUrl = await _imageUploadHelper.UploadImageAsync(viewModel.ClientImage, "clients");

            if (!string.IsNullOrEmpty(imageUrl))
            {
                viewModel.ImageUrl = imageUrl;
            }
        }

        // Map the ViewModel to the FormData and include Address
        var formMapped = viewModel.MapTo<AddClientFormData>();

        // Map Address correctly
        if (viewModel.Address != null)
        {
            formMapped.Address = new UserAddressFormData
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

        var result = await _clientService.CreateClientAsync(formMapped);
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return Problem("Unable to submit data.");
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
            formMapped.Address = new UserAddressFormData
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