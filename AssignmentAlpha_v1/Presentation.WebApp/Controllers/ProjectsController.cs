using System.Linq;
using Business.DTOs;
using Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

// [Route("projects")]
public class ProjectsController : Controller
{
    // [Route("")]
    [Route("projects")]
    public IActionResult Projects()
    {
        return View();
    }
    
    
    // Https://domain.com/projects/add
    /*[Route("add")]
    public IActionResult AddProject()
    {
        return View();
    }*/
    
    [HttpPost]
    public IActionResult AddProject(ProjectRegistrationForm form)
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
    public IActionResult EditProject(ProjectUpdateForm form)
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