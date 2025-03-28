using System.ComponentModel.DataAnnotations;
using Domain.DTOs;

namespace Presentation.WebApp.ViewModels.Logins;

public class MemberLoginViewModel
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Email Address", Prompt = "Enter your email address.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
    
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter a password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    // [Display(Name = "Remember Me", Prompt = "Remember me.")]
    public bool RememberMe { get; set; }
    
    public static implicit operator MemberLoginForm(MemberLoginViewModel model)
    {
        return model == null
            ? null!
            : new MemberLoginForm
            {
                Email = model.Email,
                Password = model.Password
            };
    }
}