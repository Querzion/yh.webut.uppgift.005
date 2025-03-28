using Domain.DTOs;
using Domain.DTOs.Adds;
using Domain.DTOs.Registrations;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(MemberLoginForm loginForm);
    Task<bool> SignUpAsync(MemberSignUpForm registrationForm);
    Task<bool> AddMemberAsync(AddMemberForm addMemberForm);
    Task LogoutAsync();
}