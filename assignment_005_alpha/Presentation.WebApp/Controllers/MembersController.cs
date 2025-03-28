using Business.Interfaces;
using Domain.DTOs.Edits;
using Domain.DTOs.Registrations;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.Logins;
using Presentation.WebApp.ViewModels.Registrations;
using Presentation.WebApp.ViewModels.SignUps;

namespace Presentation.WebApp.Controllers;

public class MembersController(IMemberService memberService) : Controller
{
    private readonly IMemberService _memberService = memberService;

    #region NEEDS FIXING! No ID is inserted, and the method is not created.

        [HttpPost]
        public async Task<IActionResult> EditMember(string id, EditMemberViewModel editMemberForm)
        {
            
            if (ModelState.IsValid)
            {
                editMemberForm.Id = id;
                
                var result = await _memberService.UpdateMemberAsync(editMemberForm);
                if (result)
                {
                    return Redirect("~/admin/members");
                }
            }
                
            var viewModel = new MembersViewModel
            {
                Title = "Team Members",
                Members = await _memberService.GetMembersAsync() ?? new List<Member>(),
                RegistrationForm = new MemberSignUpViewModel(),
                AddMember = new AddMemberViewModel(),
                EditMember = editMemberForm,
                Login = new MemberLoginViewModel()
            };
                
            return View("~/Views/Admin/Members.cshtml", viewModel);
        }

    #endregion
    
    #region Tips & Trix - 005

        // public async Task<IActionResult> Index()
        // {
        //     var viewModel = new MembersViewModel
        //     {
        //         Title = "Members",
        //         Members = await _memberService.GetMembersAsync(),
        //         RegistrationForm = new()
        //     };
        //     
        //     return View(viewModel);
        // }
        
        [HttpPost]
        public async Task<IActionResult> AddMember(AddMemberViewModel addMemberForm)
        {
            if (ModelState.IsValid)
            {
                var result = await _memberService.AddMemberAsync(addMemberForm);
                if (result)
                {
                    return Redirect("~/admin/members");
                }
            }
            
            var viewModel = new MembersViewModel
            {
                Title = "Team Members",
                Members = await _memberService.GetMembersAsync(),
                AddMember = addMemberForm,
                RegistrationForm = new MemberSignUpViewModel(),
                EditMember = new EditMemberViewModel(),
                Login = new MemberLoginViewModel()
            };
            
            return View("~/Views/Admin/Members.cshtml", viewModel);
        }
        
        // [HttpPost]
        // public async Task<IActionResult> EditMember(MemberEditViewModel editModel)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var result = await _memberService.EditMemberAsync(editModel);
        //         if (result)
        //         {
        //             return RedirectToAction("Index");
        //         }
        //     }
        //     
        //     var viewModel = new MembersViewModel
        //     {
        //         Title = "Members",
        //         Members = await _memberService.GetMembersAsync(),
        //         EditMemberForm = registrationForm
        //     };
        //     
        //     return View("Index", viewModel);
        // }

    #endregion
    
    #region Tips & Trix - 001

        // [HttpPost]
        // public IActionResult AddMember(MemberRegistrationForm form)
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
        //     // Send data to clientService
        //     // var result = await _clientService.AddMemberAsync(form);
        //     // if (result)
        //     // {
        //     //     return Ok(new { success = true });
        //     // }
        //     // else
        //     // {
        //     //     return Problem("Unable to submit data.");
        //     // }
        //     
        //     return Ok(new { success = true });
        // }
        
        // [HttpPost]
        // public IActionResult EditMember(MemberUpdateForm form)
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
        //     // Send data to clientService
        //     // var result = await _clientService.UpdateMemberAsync(form);
        //     // if (result)
        //     // {
        //     //     return Ok(new { success = true });
        //     // }
        //     // else
        //     // {
        //     //     return Problem("Unable to submit data.");
        //     // }
        //     
        //     return Ok(new { success = true });
        // }

    #endregion
}