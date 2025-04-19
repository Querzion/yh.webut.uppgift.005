using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.ViewModels.ListItems;

public class ProjectListItemViewModel
{
    public string Id { get; set; } = null!;

    [Display(Name = "Project Name")]
    [DataType(DataType.Text)]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Display(Name = "Budget")]
    public decimal? Budget { get; set; }

    [Display(Name = "Project Image")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Client Name")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Project Status")]
    public string Status { get; set; } = null!;

    [Display(Name = "Assigned User")]
    public string UserName { get; set; } = null!;
}