using System.Diagnostics;
using System.Linq.Expressions;
using Business.Factories;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IUserService
{
    Task<UserServiceResult> GetUsersAsync();
    Task<UserServiceResult> AddUserToRole(string userId, string roleName);
    Task<UserServiceResult> CreateUserAsync(SignUpFormData formData, string roleName = "User");

    Task<UserServiceResult> CreateUserAsync(AddMemberFormData formData, string roleName = "User", string password = "BytMig123!");
    Task<UserServiceResult> GetUserByEmailAsync(string email);
    Task<UserServiceResult> GetUserByIdAsync(string id);
    Task<UserServiceResult> UpdateUserAsync(string userId, EditMemberFormData formDataData);
    Task<string> GetDisplayNameAsync(string? username);
}

public class UserService(IUserRepository userRepository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IAddressService addressService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAddressService _addressService = addressService;
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
                
                // public async Task<UserServiceResult> CreateUserAsync(SignUpFormData formData, string roleName = "User")
                // {
                //     if (formData == null)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };
                //
                //     // Check if email exists
                //     var existsResult = await _userRepository.ExistsAsync(u => u.NormalizedEmail == formData.Email.ToUpperInvariant());
                //     if (existsResult.Succeeded)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 409, Error = "Email already exists." };
                //
                //     try
                //     {
                //         // Map form data to AppUser using the extension
                //         var userEntity = formData.MapTo<AppUser>(); // Uses your MapTo extension
                //
                //         // Ensure UserName is assigned if it's not already mapped
                //         userEntity.UserName = formData.Email;  // Use Email or another unique identifier
                //
                //         // Create the user
                //         var result = await _userManager.CreateAsync(userEntity, formData.Password);
                //         if (result.Succeeded)
                //         {
                //             // Add to role
                //             var addToRoleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                //             if (addToRoleResult.Succeeded)
                //             {
                //                 return new UserServiceResult { Succeeded = true, StatusCode = 201 };
                //             }
                //             else
                //             {
                //                 return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "User created, but not added to role." };
                //             }
                //         }
                //         else
                //         {
                //             return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
                //         }
                //     }
                //     catch (Exception ex)
                //     {
                //         Debug.WriteLine(ex.Message);
                //         return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                //     }
                // }

            #endregion

            #region With Transaction Management (ChatGPT help for quick creation)

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
                        // Begin transaction
                        await _userRepository.BeginTransactionAsync();

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
                                await _userRepository.CommitTransactionAsync();
                                return new UserServiceResult { Succeeded = true, StatusCode = 201 };
                            }
                            else
                            {
                                await _userRepository.RollbackTransactionAsync();
                                return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "User created, but not added to role." };
                            }
                        }
                        else
                        {
                            await _userRepository.RollbackTransactionAsync();
                            return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
                        }
                    }
                    catch (Exception ex)
                    {
                        await _userRepository.RollbackTransactionAsync();
                        Debug.WriteLine(ex.Message);
                        return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                    }
                }
                
                // public async Task<UserServiceResult> CreateUserAsync(AddMemberFormData formData, string roleName = "User", string password = "BytMig123!")
                // {
                //     if (formData == null)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };
                //
                //     // Check if email exists
                //     var existsResult = await _userRepository.ExistsAsync(u => u.NormalizedEmail == formData.Email.ToUpperInvariant());
                //     if (existsResult.Succeeded)
                //         return new UserServiceResult { Succeeded = false, StatusCode = 409, Error = "Email already exists." };
                //
                //     try
                //     {
                //         // Begin transaction
                //         await _userRepository.BeginTransactionAsync();
                //         
                //         var userEntity = UserFactory.CreateFromAddMemberForm(formData); 
                //
                //         // Ensure UserName is assigned if it's not already mapped
                //         userEntity.UserName = formData.Email;  // Use Email or another unique identifier
                //         
                //         if (formData.Image != null)
                //         {
                //             var imageEntity = formData.Image.MapTo<ImageEntity>();  // Map ImageFormData to ImageEntity
                //
                //             // Manually set the AltText if needed
                //             imageEntity.AltText = $"{formData.FirstName} {formData.LastName}'s profile picture";
                //
                //             // Assign it to the userEntity.Image property
                //             userEntity.Image = imageEntity;
                //         }
                //
                //         if (formData.Address != null)
                //             userEntity.Address = formData.Address.MapTo<AddressEntity>();
                //
                //         // Create the user
                //         var result = await _userManager.CreateAsync(userEntity, password);
                //         if (result.Succeeded)
                //         {
                //             // Add to role
                //             var addToRoleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                //             if (addToRoleResult.Succeeded)
                //             {
                //                 await _userRepository.CommitTransactionAsync();
                //                 return new UserServiceResult { Succeeded = true, StatusCode = 201 };
                //             }
                //             else
                //             {
                //                 await _userRepository.RollbackTransactionAsync();
                //                 return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "User created, but not added to role." };
                //             }
                //         }
                //         else
                //         {
                //             await _userRepository.RollbackTransactionAsync();
                //             return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
                //         }
                //     }
                //     catch (Exception ex)
                //     {
                //         await _userRepository.RollbackTransactionAsync();
                //         Debug.WriteLine(ex.Message);
                //         return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                //     }
                // }
                
                public async Task<UserServiceResult> CreateUserAsync(AddMemberFormData formData, string roleName = "User", string password = "BytMig123!")
                {
                    if (formData == null)
                        return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };

                    // Check if email exists
                    var existsResult = await _userRepository.ExistsAsync(u => u.NormalizedEmail == formData.Email.ToUpperInvariant());
                    if (existsResult.Succeeded)
                        return new UserServiceResult { Succeeded = false, StatusCode = 409, Error = "Email already exists." };

                    try
                    {
                        // Begin transaction
                        await _userRepository.BeginTransactionAsync();
                        
                        var userEntity = UserFactory.CreateFromAddMemberForm(formData); // Map form data to user entity

                        // Ensure UserName is assigned if it's not already mapped
                        userEntity.UserName = formData.Email;

                        // Handle profile image if provided
                        if (formData.Image != null)
                        {
                            var imageEntity = formData.Image.MapTo<ImageEntity>();  // Map ImageFormData to ImageEntity
                            imageEntity.AltText = $"{formData.FirstName} {formData.LastName}'s profile picture";
                            userEntity.Image = imageEntity;
                        }

                        // Handle address
                        if (formData.Address != null)
                        {
                            // Call GetOrCreateAddressAsync to get or create the address
                            var addressResult = await _addressService.GetOrCreateAddressAsync(formData.Address);

                            // If the address creation or retrieval failed, return an error
                            if (!addressResult.Succeeded)
                            {
                                await _userRepository.RollbackTransactionAsync();
                                return new UserServiceResult { Succeeded = false, StatusCode = addressResult.StatusCode, Error = addressResult.Error };
                            }

                            // Map the address to AddressEntity before assigning it to userEntity
                            userEntity.Address = addressResult.Result?.FirstOrDefault()?.MapTo<AddressEntity>();
                        }

                        // Create the user
                        var result = await _userManager.CreateAsync(userEntity, password);
                        if (result.Succeeded)
                        {
                            // Add user to role
                            var addToRoleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                            if (addToRoleResult.Succeeded)
                            {
                                await _userRepository.CommitTransactionAsync();
                                return new UserServiceResult { Succeeded = true, StatusCode = 201 };
                            }
                            else
                            {
                                await _userRepository.RollbackTransactionAsync();
                                return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "User created, but not added to role." };
                            }
                        }
                        else
                        {
                            await _userRepository.RollbackTransactionAsync();
                            return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
                        }
                    }
                    catch (Exception ex)
                    {
                        await _userRepository.RollbackTransactionAsync();
                        Debug.WriteLine(ex.Message);
                        return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                    }
                }

            #endregion

        #endregion
    
    
        #region Read

            // public async Task<UserServiceResult> GetUsersAsync()
            // {
            //     var result = await _userRepository.GetAllAsync(
            //         includes: [x => x.Image!] 
            //     );
            //     return result.MapTo<UserServiceResult>();
            // }
            
            public async Task<UserServiceResult> GetUsersAsync()
            {
                var response = await _userRepository.GetAllAsync(
                    orderByDecending: true,
                    sortBy: s => s.FirstName,
                    where: null,
                    includes: new Expression<Func<AppUser, object>>[]
                    {
                        x => x.Image,
                        x => x.Address
                    }
                );

                // Assuming you want to map the response to a service result of User
                var users = response.Result.Select(appUser => appUser.MapTo<User>()).ToList();

                return new UserServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = users // Result is already of type IEnumerable<User>
                };
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
            
            public async Task<UserServiceResult> GetUserByIdAsync(string id)
            {
                var result = await _userRepository.GetAsync(u => u.Id == id);

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

        #region Update (ChatGPT)

            public async Task<UserServiceResult> UpdateUserAsync(string userId, EditMemberFormData formDataData)
            {
                if (formDataData == null)
                    return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };

                // Get the raw AppUser entity
                var userResult = await _userRepository.GetEntityAsync(u => u.Id == userId,
                    x => x.Address, x => x.ImageId); // Include any related entities you may want to update

                if (!userResult.Succeeded || userResult.Result == null)
                    return new UserServiceResult { Succeeded = false, StatusCode = 404, Error = "User not found." };

                var existingUser = userResult.Result;

                try
                {
                    // Begin transaction
                    await _userRepository.BeginTransactionAsync();

                    // Update the existing user with the form data
                    UserFactory.UpdateFromEditMemberForm(existingUser, formDataData);

                    // Update the user in the database
                    var result = await _userManager.UpdateAsync(existingUser);
                    if (result.Succeeded)
                    {
                        await _userRepository.CommitTransactionAsync();
                        return new UserServiceResult { Succeeded = true, StatusCode = 200 };
                    }
                    else
                    {
                        await _userRepository.RollbackTransactionAsync();
                        return new UserServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Error = string.Join("; ", result.Errors.Select(e => e.Description))
                        };
                    }
                }
                catch (Exception ex)
                {
                    await _userRepository.RollbackTransactionAsync();
                    return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                }
            }


        #endregion

        #region Delete (ChatGPT)

            public async Task<UserServiceResult> DeleteUserAsync(string userId)
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return new UserServiceResult { Succeeded = false, StatusCode = 400, Error = "User ID cannot be null or empty." };

                try
                {
                    await _userRepository.BeginTransactionAsync();

                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        await _userRepository.RollbackTransactionAsync();
                        return new UserServiceResult { Succeeded = false, StatusCode = 404, Error = "User not found." };
                    }

                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        await _userRepository.CommitTransactionAsync();
                        return new UserServiceResult { Succeeded = true, StatusCode = 200 };
                    }
                    else
                    {
                        await _userRepository.RollbackTransactionAsync();
                        return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = "Failed to delete user." };
                    }
                }
                catch (Exception ex)
                {
                    await _userRepository.RollbackTransactionAsync();
                    Debug.WriteLine(ex.Message);
                    return new UserServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
                }
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