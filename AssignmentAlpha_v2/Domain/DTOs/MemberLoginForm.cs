using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs;

public class MemberLoginForm
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Email Address", Prompt = "Enter your email address.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter a password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}