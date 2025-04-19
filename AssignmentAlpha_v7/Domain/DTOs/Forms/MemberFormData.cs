using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Adds;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Forms;

public class MemberFormData
{
    [Display(Name = "Member Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? UserImage { get; set; }
    
    public ImageFormData? Image { get; set; }
    
    public string? ImageId { get; set; }
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "First Name", Prompt = "Your first name.")]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Last Name", Prompt = "Your last name.")]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;
    
    
    [Display(Name = "Job Title", Prompt = "Your job title.")]
    [DataType(DataType.Text)]
    public string? JobTitle { get; set; }
    
    
    [Display(Name = "Email", Prompt = "Your email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;
    
    
    [Display(Name = "Phone Number", Prompt = "Your phone number.")]
    [RegularExpression(@"^\+?\d{1,4}\s?\d{2,4}(\s?-?\s?\d{2,4}){2,3}$", 
        ErrorMessage = "Invalid phone number format.")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter a password.")]
    [RegularExpression(@"^(?=.*[A-Ö])(?=.*[a-ö])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$", 
        ErrorMessage = "Invalid Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    public AddressFormData? Address { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
}