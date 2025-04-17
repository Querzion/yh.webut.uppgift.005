using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.WebApp.ViewModels.Forms;

public class ProjectFormViewModel
{
    [Display(Name = "Project Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }

    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [Display(Name = "Project Name", Prompt = "Enter the project name")]
    [DataType(DataType.Text)]
    public string ProjectName { get; set; } = null!;
    
    [Display(Name = "Description", Prompt = "Enter project description")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Project description is required")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Client name is required.")]
    [Display(Name = "Client Name", Prompt = "Enter the client's name")]
    [DataType(DataType.Text)]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Budget", Prompt = "Enter project budget")]
    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date", Prompt = "Select the end date (if known)")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Client selection is required.")]
    [Display(Name = "Client", Prompt = "-- Select a Client --")]
    public string ClientId { get; set; } = null!;

    [Required(ErrorMessage = "Status is required")]
    [Display(Name = "Status")]
    public int StatusId { get; set; }

    [Required(ErrorMessage = "Owner is required")]
    [Display(Name = "Owner")]
    public string UserId { get; set; } = null!;
    
    public List<int> SelectedUserIds { get; set; } = [];

    // These are to populate dropdowns in the View
    public List<SelectListItem> Clients { get; set; } = [];
    public List<SelectListItem> Statuses { get; set; } = [];
    public List<SelectListItem> Users { get; set; } = [];
    
    public List<SelectListItem> ClientOptions { get; set; } = new List<SelectListItem>();
}