using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Domain.Models;

public class User
{
    public string Id { get; set; } = null!;

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Email")] 
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
    
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Profile Image")]
    public virtual Image? ProfileImage { get; set; }
    
    [Display(Name = "Address")]
    public virtual UserAddress? Address { get; set; }
}