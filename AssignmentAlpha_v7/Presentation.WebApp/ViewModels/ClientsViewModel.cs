using Domain.Models;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.ViewModels;

public class ClientsViewModel
{
    public string Title { get; set; } = null!;

    public IEnumerable<Client>? Clients { get; set; } = [];

    public AddClientViewModel AddClient { get; set; } = new();
        
    public EditClientViewModel EditClient { get; set; } = new();
}