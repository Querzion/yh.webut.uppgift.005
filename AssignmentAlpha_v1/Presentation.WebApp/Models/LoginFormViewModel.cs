using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models;

public class LoginFormViewModel
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Email Address", Prompt = "Enter your email address.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter a password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}