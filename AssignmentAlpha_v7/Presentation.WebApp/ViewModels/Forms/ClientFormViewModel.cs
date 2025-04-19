using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Adds;
using Domain.DTOs.Forms;

namespace Presentation.WebApp.ViewModels.Forms;

public class ClientFormViewModel
{
    [Display(Name = "Client Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? ClientImage { get; set; }

    public ImageFormData? Image { get; set; }
    
    public string? ImageUrl { get; set; }

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Client name is required.")]
    [StringLength(200, ErrorMessage = "Client name cannot exceed 200 characters.")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid email format.")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\+?\d{1,4}\s?\d{2,4}(\s?-?\s?\d{2,4}){2,3}$", 
        ErrorMessage = "Invalid phone number format.")]
    public string? PhoneNumber { get; set; }

    [StringLength(36)]
    public string? AddressId { get; set; }
    public AddressFormData? Address { get; set; }
    
    [Display(Name = "Location", Prompt = "Enter location")]
    [DataType(DataType.Text)]
    public string? Location =>
        Address == null ? null :
            string.Join(", ", new[]
            {
                Address.StreetName,
                Address.PostalCode,
                Address.City
            }.Where(part => !string.IsNullOrWhiteSpace(part)));
}