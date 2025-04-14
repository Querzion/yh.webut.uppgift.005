using System.ComponentModel.DataAnnotations;
using Domain.DTOs;
using Domain.DTOs.Registrations;

namespace Presentation.WebApp.ViewModels.SignUps;

public class MemberSignUpViewModel
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
    
    
    [Display(Name = "Phone Number", Prompt = "Enter your phone number.")]
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
    
    
    [Required(ErrorMessage = "Required")]
    [Compare(nameof(Password), ErrorMessage = "Password must be confirmed.")]
    [Display(Name = "Confirm Password", Prompt = "Confirm the password.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;
    
    
    // [Required(ErrorMessage = "Required")]
    [Display(Name = "Terms & Conditions", Prompt = "I Accept the terms and conditions.")]
    [Range(typeof(bool), "true", "true", ErrorMessage="You must accept the terms and conditions to use this site.")]
    public bool TermsAndConditions { get; set; }
    
    // Alternative ViewModel to DTO mapping instead of in the controller.
    public static implicit operator MemberSignUpForm(MemberSignUpViewModel model)
    {
        return model == null
            ? null!
            : new MemberSignUpForm
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password
            };
    }
}