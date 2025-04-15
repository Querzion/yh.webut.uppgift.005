using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Domain.DTOs;

/// <summary>
/// Validation here was added as a double layer if registrations were ever to happen through an API and not only the
/// ViewModel UI registration process. Source: https://chatgpt.com/share/67ec744c-f174-8005-89c9-9aea91e92648
/// </summary>

public class SignUpFormData
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "First Name", Prompt = "Enter your first name.")]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Last Name", Prompt = "Enter your last name.")]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Email Address", Prompt = "Enter your email address.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter a password.")]
    [RegularExpression(@"^(?=.*[A-Ö])(?=.*[a-ö])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$", 
        ErrorMessage = "Invalid Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}