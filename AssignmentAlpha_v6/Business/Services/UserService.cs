using System.Diagnostics;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IUserService
{
    Task<UserServiceResult> GetUsersAsync();
    Task<UserServiceResult> AddUserToRole(string userId, string roleName);
    Task<UserServiceResult> CreateUserAsync(SignUpFormData formData, string roleName = "User");
    Task<UserServiceResult> GetUserByEmailAsync(string email);
    Task<string> GetDisplayNameAsync(string? username);
}

public class UserService(IUserRepository userRepository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    #region CRUD

        #region Create

            #region Without Transaction Management

                // public async Task<UserServiceResult> CreateUserAsync(SignUpFormData formData, string roleName = "User")
                // {
                //     if (formData == null)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };
                //         
                //     var existsResult = await _userRepository.ExistsAsync(u => u.Email == formData.Email);
                //     if (existsResult.Succeeded)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 409, Error = "Email already exists." };
                //
                //     try
                //     {
                //         var userEntity = formData.MapTo<AppUser>();
                //             
                //         var result = await _userManager.CreateAsync(userEntity, formData.Password);
                //         if (result.Succeeded)
                //         {
                //             var addToRoleResult = await AddUserToRole(userEntity.Id, roleName);
                //             return result.Succeeded
                //                 ? new UserServiceResult { Succeeded = true, StatusCode = 201 }
                //                 : new UserServiceResult { Succeeded = false, StatusCode = 201, Error = "User created, but not added to role." };
                //         }
                //             
                //         return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user."};
                //     }
                //     catch (Exception ex)
                //     {
                //         Debug.WriteLine(ex.Message);
                //         return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message};
                //     }
                // }
                
                public async Task<UserServiceResult> CreateUserAsync(SignUpFormData formData, string roleName = "User")
                {
                    if (formData == null)
                        return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };
    
                    // Check if email exists
                    var existsResult = await _userRepository.ExistsAsync(u => u.NormalizedEmail == formData.Email.ToUpperInvariant());
                    if (existsResult.Succeeded)
                        return new UserServiceResult { Succeeded = false, StatusCode = 409, Error = "Email already exists." };

                    try
                    {
                        // Map form data to AppUser using the extension
                        var userEntity = formData.MapTo<AppUser>(); // Uses your MapTo extension

                        // Ensure UserName is assigned if it's not already mapped
                        userEntity.UserName = formData.Email;  // Use Email or another unique identifier

                        // Create the user
                        var result = await _userManager.CreateAsync(userEntity, formData.Password);
                        if (result.Succeeded)
                        {
                            // Add to role
                            var addToRoleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                            if (addToRoleResult.Succeeded)
                            {
                                return new UserServiceResult { Succeeded = true, StatusCode = 201 };
                            }
                            else
                            {
                                return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "User created, but not added to role." };
                            }
                        }
                        else
                        {
                            return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                    }
                }

            #endregion

            #region With Transaction Management (ChatGPT help for quick creation)

                // public async Task<UserServiceResult> CreateUserAsync(SignUpFormData formData, string roleName = "User")
                // {
                //     if (formData == null)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };
                //
                //     var existsResult = await _userRepository.ExistsAsync(u => u.Email == formData.Email);
                //     if (existsResult.Succeeded)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 409, Error = "Email already exists." };
                //
                //     try
                //     {
                //         // Start the transaction from the repository
                //         await _userRepository.BeginTransactionAsync();
                //
                //         // Map the form data to the AppUser entity
                //         var userEntity = formData.MapTo<AppUser>();
                //
                //         // Use UserManager to create the user
                //         var result = await _userManager.CreateAsync(userEntity, formData.Password);
                //         if (result.Succeeded)
                //         {
                //             // Add the user to the role
                //             var addToRoleResult = await AddUserToRole(userEntity.Id, roleName);
                //
                //             // Commit the transaction if everything goes well
                //             if (addToRoleResult.Succeeded)
                //             {
                //                 await _userRepository.CommitTransactionAsync();
                //                 return new UserServiceResult { Succeeded = true, StatusCode = 201 };
                //             }
                //
                //             // If adding to role fails, rollback and return error
                //             await _userRepository.RollbackTransactionAsync();
                //             return new UserServiceResult { Succeeded = false, StatusCode = 201, Error = "User created, but not added to role." };
                //         }
                //
                //         // If user creation fails, rollback and return error
                //         await _userRepository.RollbackTransactionAsync();
                //         return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
                //     }
                //     catch (Exception ex)
                //     {
                //         // Rollback the transaction in case of an exception
                //         await _userRepository.RollbackTransactionAsync();
                //         Debug.WriteLine(ex.Message);
                //         return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                //     }
                // }

            #endregion

        #endregion
    
    
        #region Read

            public async Task<UserServiceResult> GetUsersAsync()
            {
                var result = await _userRepository.GetAllAsync();
                return result.MapTo<UserServiceResult>();
            }

            public async Task<UserServiceResult> GetUserByEmailAsync(string email)
            {
                var result = await _userRepository.GetAsync(u => u.Email == email);

                if (!result.Succeeded || result.Result == null)
                {
                    return new UserServiceResult
                    {
                        Succeeded = false,
                        StatusCode = result.StatusCode,
                        Error = result.Error ?? "User not found.",
                        Result = null
                    };
                }

                return new UserServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = new List<User> { result.Result }
                };
            }
            

        #endregion

    #endregion

    public async Task<UserServiceResult> AddUserToRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new UserServiceResult { Succeeded = false, StatusCode = 404, Error = "Role doesn't exist."};

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserServiceResult { Succeeded = false, StatusCode = 404, Error = "User doesn't exist."};
        
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? new UserServiceResult { Succeeded = true, StatusCode = 200 }
            : new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to add user to role."};
    }

    public async Task<string> GetDisplayNameAsync(string? username)
    {
        if (username == null)
            return "";
        
        var user = await _userManager.FindByNameAsync(username);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }
}