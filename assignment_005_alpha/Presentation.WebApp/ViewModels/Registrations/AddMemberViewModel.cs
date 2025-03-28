using System.ComponentModel.DataAnnotations;
using Data.Entities;
using Domain.DTOs.Adds;
using Domain.DTOs.Registrations;

namespace Presentation.WebApp.ViewModels.Registrations;

public class AddMemberViewModel
{
    [Display(Name = "Member Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? MemberImage { get; set; }
    public string? MemberImagePath { get; set; }
    
    
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
    
    
    [Display(Name = "Street Name", Prompt = "Your Street Name")]
    [DataType(DataType.Text)]
    public string? StreetName { get; set; }
    
    [Display(Name = "Postal Code", Prompt = "Your Postal Code")]
    [DataType(DataType.Text)]
    public string? PostalCode { get; set; }
    
    [Display(Name = "City", Prompt = "Your City")]
    [DataType(DataType.Text)]
    public string? City { get; set; }
    
    public int? SelectedDay { get; set; }
    public int? SelectedMonth { get; set; }
    public int? SelectedYear { get; set; }
    
    public DateTime? DateOfBirth { get; set; }

    public static implicit operator AddMemberForm(AddMemberViewModel model)
    {
        return model == null
            ? null!
            : new AddMemberForm
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                JobTitle = model.JobTitle,
                DateOfBirth = model.DateOfBirth,
                MemberImagePath = model.MemberImagePath,
                StreetName = model.StreetName,
                PostalCode = model.PostalCode,
                City = model.City
            };
    }
}