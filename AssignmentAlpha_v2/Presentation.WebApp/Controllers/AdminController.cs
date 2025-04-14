using Business.Interfaces;
using Domain.DTOs;
using Domain.DTOs.Registrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.ViewModels;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.Logins;
using Presentation.WebApp.ViewModels.Registrations;
using Presentation.WebApp.ViewModels.SignUps;

namespace Presentation.WebApp.Controllers;

[Authorize]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IMemberService _memberService;

    public AdminController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [Route("members")]
    public async Task<IActionResult> Members()
    {
        var viewModel = new MembersViewModel
        {
            Title = "Team Members",
            Members = await _memberService.GetMembersAsync(),
            RegistrationForm = new MemberSignUpViewModel(),
            AddMember = new AddMemberViewModel(),
            EditMember = new EditMemberViewModel(),
            Login = new MemberLoginViewModel()
         };
        
        return View(viewModel);
        
        
        // var members = await _memberService.GetMembersAsync();
        //
        // #region ChatGPT helped here.
        //
        //     var viewModel = new MembersViewModel
        //     {
        //         Members = members,
        //         AddMemberForm = new AddMemberForm(),
        //         EditMemberForm = new EditMemberForm()
        //     };
        //     return View(viewModel);
        //
        // #endregion
    }

    [Route("clients")]
    public IActionResult Clients()
    {
        return View();
    }
}