using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Domain.Models;

public class User
{
    [StringLength(36)]
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
    [StringLength(36)]
    public string? ImageId { get; set; }
    public Image? Image { get; set; }
    
    [Display(Name = "Address")]
    [StringLength(36)]
    public string? AddressId { get; set; }
    public Address? Address { get; set; }
    
    public ICollection<Project> Projects { get; set; } = [];
}