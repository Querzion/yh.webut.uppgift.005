using Data.Entities;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Registrations;
using Domain.Models;

namespace Business.Factories
{
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
                Address = form.Address != null ? new UserAddressEntity
                {
                    
                    StreetName = form.Address.StreetName,
                    City = form.Address.City,
                    PostalCode = form.Address.PostalCode,
                } : null,
                Image = form.Image != null ? new ImageEntity
                {
                    ImageUrl = form.Image.ImageUrl,
                    AltText = form.Image.AltText,
                    UploadedAt = DateTime.UtcNow,
                    // ClientId = form.Image.ClientId,
                    // ProjectId = form.Image.ProjectId,
                    // UserId = form.Image.UserId
                } : null,
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
                existingUser.Address = new UserAddressEntity
                {
                    StreetName = formData.Address.StreetName,
                    City = formData.Address.City,
                    PostalCode = formData.Address.PostalCode
                };
            }

            if (formData.Image != null)
            {
                existingUser.Image = new ImageEntity
                {
                    ImageUrl = formData.Image.ImageUrl,
                    AltText = formData.Image.AltText,
                    UploadedAt = DateTime.UtcNow,
                    // Clients = formData.Id,
                    // Projects = formData.Image.ProjectId,
                    // Users = formData.Image.UserId
                };
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
                ProfileImage = entity.Image != null ? new Image
                    {
                        Id = entity.Image.Id,
                        ImageUrl = entity.Image.ImageUrl,
                        AltText = entity.Image.AltText,
                        UploadedAt = entity.Image.UploadedAt
                    } : null,
                Address = entity.Address != null ? new UserAddress
                    {
                        StreetName = entity.Address.StreetName,
                        City = entity.Address.City,
                        PostalCode = entity.Address.PostalCode
                    } : null
            };
        }
    }
}
