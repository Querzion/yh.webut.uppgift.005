using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.ViewModels.ListItems;

public class ClientListItemViewModel
{
    public string Id { get; set; } = null!;

    [Display(Name = "Client Name")]
    [DataType(DataType.Text)]
    [Required]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Alt Text")]
    public string? AltText { get; set; }

    [Display(Name = "Status")]
    public bool IsActive { get; set; }

    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Display(Name = "Location", Prompt = "Enter location")]
    [DataType(DataType.Text)]
    public string? Location { get; set; }
}