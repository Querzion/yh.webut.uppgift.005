using Domain.Models;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(MemberLoginForm loginForm);
    Task<bool> SignUpAsync(MemberSignUpForm signUpForm);
    Task LogoutAsync();
}