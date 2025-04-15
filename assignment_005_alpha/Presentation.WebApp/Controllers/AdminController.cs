using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.ViewModels;

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
        var members = await _memberService.GetAllMembersAsync();

        #region ChatGPT helped here.
        
            var viewModel = new MembersViewModel
            {
                Members = members,
                AddMemberForm = new AddMemberForm(),
                EditMemberForm = new EditMemberForm()
            };
            return View(viewModel);
        
        #endregion
    }

    [Route("clients")]
    public IActionResult Clients()
    {
        return View();
    }
}