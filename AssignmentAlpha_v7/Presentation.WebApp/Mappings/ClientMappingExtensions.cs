using Domain.Models;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.Mappings;

public static class ClientMappingExtensions
{
    public static ClientListItemViewModel ToViewModel(this Client client)
    {
        return new ClientListItemViewModel
        {
            Id = client.Id,
            ClientName = client.ClientName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            ImageUrl = client.Image?.ImageUrl,
            AltText = client.Image?.AltText,
            IsActive = client.IsActive,
            Date = client.Date,
            Location = client.Address == null ? null :
                string.Join(", ", new[]
                {
                    client.Address.StreetName,
                    client.Address.PostalCode,
                    client.Address.City
                }.Where(x => !string.IsNullOrWhiteSpace(x)))
        };
    }

    public static IEnumerable<ClientListItemViewModel> ToViewModelList(this IEnumerable<Client> clients)
    {
        return clients.Select(c => c.ToViewModel());
    }
}