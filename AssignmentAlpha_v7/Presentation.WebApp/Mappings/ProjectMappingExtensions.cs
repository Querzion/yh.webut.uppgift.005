using Domain.Models;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.Mappings;

public static class ProjectMappingExtensions
{
    public static ProjectListItemViewModel ToViewModel(this Project project)
    {
        return new ProjectListItemViewModel
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ImageUrl = project.Image?.ImageUrl,
            ClientName = project.Client.ClientName, // Assuming you want the client name
            Status = project.Status.StatusName, // Assuming the status has a Name property
            UserName = project.User?.FirstName + " " + project.User?.LastName, // Assuming full name
        };
    }

    public static IEnumerable<ProjectListItemViewModel> ToViewModelList(this IEnumerable<Project> projects)
    {
        return projects.Select(p => p.ToViewModel());
    }
}