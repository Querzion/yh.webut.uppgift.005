using Domain.Models;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.Mappings;

public static class MemberMappingExtensions
{
    // Mapping a single User to MemberListItemViewModel
    public static MemberListItemViewModel ToViewModel(this User user)
    {
        return new MemberListItemViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            DateOfBirth = user.DateOfBirth,
            ImageUrl = user.Image?.ImageUrl,
            AltText = user.Image?.AltText,
            Address = user.Address != null 
                ? $"{user.Address.StreetName}, {user.Address.City}, {user.Address.PostalCode}" 
                : null
        };
    }

    // Mapping a list of Users to a list of MemberListItemViewModels
    public static IEnumerable<MemberListItemViewModel> ToViewModelList(this IEnumerable<User> users)
    {
        return users.Select(user => user.ToViewModel());
    }
}