using Business.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class ClientsController : Controller
{
    // Add This in later when it works properly.
    
    // private readonly IClientService _clientService;
    //
    // public ClientsController(IClientService clientService)
    // {
    //     _clientService = clientService;
    // }

    [HttpPost]
    public IActionResult AddClient(ClientRegistrationForm form)
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

        // Send data to clientService
        // var result = await _clientService.AddClientAsync(form);
        // if (result)
        // {
        //     return Ok(new { success = true });
        // }
        // else
        // {
        //     return Problem("Unable to submit data.");
        // }
        
        return Ok(new { success = true });
    }
    
    [HttpPost]
    public IActionResult EditClient(ClientUpdateForm form)
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
            
        // Send data to clientService
        // var result = await _clientService.UpdateClientAsync(form);
        // if (result)
        // {
        //     return Ok(new { success = true });
        // }
        // else
        // {
        //     return Problem("Unable to submit data.");
        // }
        
        return Ok(new { success = true });
    }
}