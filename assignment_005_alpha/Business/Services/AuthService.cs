using Business.Factories;
using Business.Interfaces;
using Data.Entities;
using Domain.DTOs;
using Domain.DTOs.Adds;
using Domain.DTOs.Registrations;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class AuthService(SignInManager<MemberEntity> signInManager, UserManager<MemberEntity> userManager) : IAuthService
{
    private readonly SignInManager<MemberEntity> _signInManager = signInManager;
    private readonly UserManager<MemberEntity> _userManager = userManager;

    public async Task<bool> LoginAsync(MemberLoginForm loginForm)
    {
        var result = await _signInManager.PasswordSignInAsync(loginForm.Email, loginForm.Password, false, false);
        return result.Succeeded;
    }

    public async Task<bool> SignUpAsync(MemberSignUpForm signUpForm)
    {
        var memberEntity = MemberFactory.ToEntity(signUpForm);
        
        // Memo: To add the address through a service, you need to find the Id of the person you are editing/creating
        // then add the extended information.
        var result = await _userManager.CreateAsync(memberEntity, signUpForm.Password);
        return result.Succeeded;
    }
    
    public async Task<bool> AddMemberAsync(AddMemberForm addMemberForm)
    {
        var memberEntity = MemberFactory.ToEntity(addMemberForm);
        
        // Memo: To add the address through a service, you need to find the Id of the person you are editing/creating
        // then add the extended information.
        var result = await _userManager.CreateAsync(memberEntity, "BytMig123!");
        return result.Succeeded;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}