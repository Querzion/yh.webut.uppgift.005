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

    [Display(Name = "Avatar Image")]
    public string? ImageId { get; set; }
    public virtual Image? Image { get; set; }
    
    [Display(Name = "Address")]
    public string? AddressId { get; set; }
    public virtual Address? Address { get; set; }
    
    public virtual ICollection<Project> Projects { get; set; } = [];
}