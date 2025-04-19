using Business.Services;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.ViewModels;

public class ProjectsViewModel(IClientService clientService)
{
    private readonly IClientService _clientService = clientService;

    public string Title { get; set; } = null!;
    
    public ProjectEntity? SelectedProject { get; set; }
    public IEnumerable<ProjectListItemViewModel> Projects { get; set; } = []; // Initialize with new List
    public IEnumerable<SelectListItem> ClientOptions { get; set; } = new List<SelectListItem>(); // Initialize with new List
    public AddProjectViewModel AddProject { get; set; } = new AddProjectViewModel();
    public EditProjectViewModel EditProject { get; set; } = new EditProjectViewModel();

    public async Task PopulateClientOptionsAsync()
    {
        // Fetch clients from the client service
        var result = await _clientService.GetClientsAsync();

        if (!result.Succeeded)
            return;

        // Ensure clients list is populated
        var clients = result.Result ?? new List<Client>(); // Initialize with empty list if result is null

        // Populate ClientOptions for SelectList in the view
        ClientOptions = clients.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.ClientName
        }).ToList();
    }
}