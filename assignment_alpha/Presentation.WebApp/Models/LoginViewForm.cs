using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models;

public class LoginViewForm
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Email Address", Prompt = "Enter your email address.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter a password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Remember Me", Prompt = "Remember me")]
    public bool RememberMe { get; set; } = false;
}