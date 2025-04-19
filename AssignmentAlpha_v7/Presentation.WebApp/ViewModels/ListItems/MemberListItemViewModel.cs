using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.ViewModels.ListItems;

public class MemberListItemViewModel
{
    public string Id { get; set; } = null!;
        
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public string? ImageUrl { get; set; } // If you need to pass image URL to view
    public string? AltText { get; set; }
    public string? Address { get; set; } // You can pass a combined address if you want
}