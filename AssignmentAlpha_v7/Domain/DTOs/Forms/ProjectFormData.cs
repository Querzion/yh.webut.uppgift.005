using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Adds;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Forms;

public class ProjectFormData
{
    [Display(Name = "Project Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }
    
    public ImageFormData? Image { get; set; }
    
    public string? ImageId { get; set; }
    
    [Display(Name = "Project Name", Prompt = "Enter project name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Project name is required")]
    public string ProjectName { get; set; } = null!;
    
    [Display(Name = "Description", Prompt = "Enter project description")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Project description is required")]
    public string? Description { get; set; }
    
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
    
    [Display(Name = "End Date", Prompt = "Select the end date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
    
    [Display(Name = "Budget", Prompt = "Enter the project budget")]
    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }
    
    [Display(Name = "Client", Prompt = "Select a client")]
    [Required(ErrorMessage = "Client is required")]
    public string ClientId { get; set; } = null!;
    
    [Display(Name = "User", Prompt = "Select a user")]
    [Required(ErrorMessage = "User is required")]
    public string UserId { get; set; } = null!;
    
    [Display(Name = "Status", Prompt = "Select the project status")]
    [Required(ErrorMessage = "Status is required")]
    public int StatusId { get; set; }
}