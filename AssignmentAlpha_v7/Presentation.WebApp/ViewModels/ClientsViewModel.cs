using Domain.Models;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.ViewModels;

public class ClientsViewModel
{
    public string Title { get; set; } = null!;

    public IEnumerable<ClientListItemViewModel>? Clients { get; set; } = [];

    public AddClientViewModel AddClient { get; set; } = new();
        
    public EditClientViewModel EditClient { get; set; } = new();
}