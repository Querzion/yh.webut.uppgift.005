using Data.Entities;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Registrations;
using Domain.Extensions;
using Domain.Models;

namespace Business.Factories;

public static class UserFactory
{
    public static AppUser CreateFromRegistrationForm(UserRegistrationForm form, string imageId = null!)
    {
        return new AppUser
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            UserName = form.Email,
            PhoneNumber = form.PhoneNumber,
            JobTitle = form.JobTitle
        };
    }

    public static AppUser CreateFromAddMemberForm(AddMemberFormData form)
    {
        return new AppUser
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            UserName = form.Email,
            PhoneNumber = form.PhoneNumber,
            JobTitle = form.JobTitle,
            Address = form.Address?.MapTo<UserAddressEntity>(),
            Image = form.Image?.MapTo<ImageEntity>(),
            ImageId = form.ImageId,
            DateOfBirth = form.DateOfBirth
        };
    }
    
    public static void UpdateFromEditMemberForm(AppUser existingUser, EditMemberFormData formData)
    {
        existingUser.FirstName = formData.FirstName;
        existingUser.LastName = formData.LastName;
        existingUser.Email = formData.Email;
        existingUser.PhoneNumber = formData.PhoneNumber;
        existingUser.JobTitle = formData.JobTitle;
        existingUser.DateOfBirth = formData.DateOfBirth;

        if (formData.Address != null)
        {
            existingUser.Address = formData.Address.MapTo<UserAddressEntity>(); // Map Address if available
        }

        if (formData.Image != null)
        {
            existingUser.Image = formData.Image.MapTo<ImageEntity>(); // Map Image if available
            existingUser.ImageId = formData.ImageId; // Ensure ImageId is updated if provided
        }
    }

    public static User Create(AppUser entity)
    {
        return new User
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email!,
            PhoneNumber = entity.PhoneNumber,
            JobTitle = entity.JobTitle,
            DateOfBirth = entity.DateOfBirth,
            ProfileImage = entity.Image?.MapTo<Image>(),
            Address = entity.Address?.MapTo<UserAddress>()
        };
    }
}