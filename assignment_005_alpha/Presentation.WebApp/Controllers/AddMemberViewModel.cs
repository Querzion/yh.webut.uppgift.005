using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Controllers;

public class AddMemberViewModel
{
    [Display(Name = "Member Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? MemberImage { get; set; }
    
    [Display(Name = "Member Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Required")]
    public string MemberName { get; set; } = null!;
    
    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;
    
    [Display(Name = "Location", Prompt = "Enter location")]
    [DataType(DataType.Text)]
    public string? Location { get; set; }
    
    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
}