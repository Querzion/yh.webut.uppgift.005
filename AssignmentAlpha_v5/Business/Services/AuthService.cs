using Business.Models;
using Data.Entities;
using Domain.DTOs;
using Domain.DTOs.Authentication;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthServiceResult> SignInAsync(SignInFormData formData);
    Task<AuthServiceResult> ExternalSignInAsync(AppUser user);
    Task<AuthServiceResult> SignUpAsync(SignUpFormData formData);
    Task<AuthServiceResult> SignOutAsync();
}

public class AuthService(IUserService userService, SignInManager<AppUser> signInManager, INotificationService notificationService, UserManager<AppUser> userManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly INotificationService _notificationService = notificationService;

    #region SignIn - With Notifications

        public async Task<AuthServiceResult> SignInAsync(SignInFormData formData)
        {
            if (formData == null)
            {
                return new AuthServiceResult
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "Not all required fields are supplied."
                };
            }

            var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(formData.Email);

                if (user != null)
                {
                    var notificationEntity = new NotificationEntity
                    {
                        Message = $"{user.FirstName} {user.LastName} signed in.",
                        NotificationTypeId = 1
                    };

                    await _notificationService.AddNotificationAsync(notificationEntity, user.Id);
                }

                return new AuthServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200
                };
            }

            return new AuthServiceResult
            {
                Succeeded = false,
                StatusCode = 401,
                Error = "Invalid email or password."
            };
        }

    #endregion
    
    #region SignIn - Without Notifications

        // public async Task<AuthServiceResult> SignInAsync(SignInFormData formData)
        // {
        //     if (formData == null)
        //         return new AuthServiceResult
        //         {
        //             Succeeded = false, 
        //             StatusCode = 400, 
        //             Error = "Not all required fields are supplied."
        //         };
        //     
        //     var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);
        //     return result.Succeeded
        //         ? new AuthServiceResult { Succeeded = true, StatusCode = 200 }
        //         : new AuthServiceResult { Succeeded = false, StatusCode = 401, Error = "Invalid email or password." };
        // }

    #endregion
    
    // public async Task<AuthServiceResult> SignInAsync(SignInFormData formData)
    // {
    //     if (formData == null)
    //         return new AuthServiceResult
    //         { 
    //             Succeeded = false, 
    //             StatusCode = 400, 
    //             Error = "Not all required fields are supplied." 
    //         };
    //
    //     // Attempt to sign in using the provided credentials
    //     var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);
    //
    //     if (result.Succeeded)
    //     {
    //         return new AuthServiceResult 
    //         { 
    //             Succeeded = true, 
    //             StatusCode = 200 
    //         };
    //     }
    //
    //     // Handling additional failure cases
    //     if (result.IsLockedOut)
    //     {
    //         return new AuthServiceResult 
    //         { 
    //             Succeeded = false, 
    //             StatusCode = 403, 
    //             Error = "Your account is locked." 
    //         };
    //     }
    //
    //     if (result.IsNotAllowed)
    //     {
    //         return new AuthServiceResult 
    //         { 
    //             Succeeded = false, 
    //             StatusCode = 403, 
    //             Error = "Your account is not allowed to sign in." 
    //         };
    //     }
    //
    //     // If it's a failed login attempt
    //     return new AuthServiceResult 
    //     { 
    //         Succeeded = false, 
    //         StatusCode = 401, 
    //         Error = "Invalid email or password." 
    //     };
    // }

    #region External SignIn - With Notification

        public async Task<AuthServiceResult> ExternalSignInAsync(AppUser user)
        {
            if (user == null)
                return new AuthServiceResult
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "User is null."
                };

            await _signInManager.SignInAsync(user, isPersistent: false);

            try
            {
                var notificationEntity = new NotificationEntity
                {
                    Message = $"{user.FirstName} {user.LastName} signed in.",
                    NotificationTypeId = 1
                };

                await _notificationService.AddNotificationAsync(notificationEntity, user.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification failed: {ex.Message}");
            }
            
            return new AuthServiceResult
            {
                Succeeded = true,
                StatusCode = 200
            };
        }
    
    #endregion

    #region External SignIn - Without Notification

        // public async Task<AuthServiceResult> ExternalSignInAsync(AppUser user)
        // {
        //     if (user == null)
        //         return new AuthServiceResult
        //             { Succeeded = false, StatusCode = 400, Error = "User is null." };
        //
        //     await _signInManager.SignInAsync(user, isPersistent: false);
        //     return new AuthServiceResult { Succeeded = true, StatusCode = 200 };
        // }

    #endregion

    public async Task<AuthServiceResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthServiceResult
                { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _userService.CreateUserAsync(formData);
        return result.Succeeded
            ? new AuthServiceResult { Succeeded = true, StatusCode = 201 }
            : new AuthServiceResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
    
    public async Task<AuthServiceResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthServiceResult { Succeeded = true, StatusCode = 200 };
    }
}