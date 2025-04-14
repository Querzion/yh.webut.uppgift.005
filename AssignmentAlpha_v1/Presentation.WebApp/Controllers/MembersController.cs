using System.Linq;
using Business.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class MembersController : Controller
{
    [HttpPost]
    public IActionResult AddMember(MemberRegistrationForm form)
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
        // var result = await _clientService.AddMemberAsync(form);
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
    public IActionResult EditMember(MemberUpdateForm form)
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
        // var result = await _clientService.UpdateMemberAsync(form);
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