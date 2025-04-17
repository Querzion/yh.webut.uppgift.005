using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Adds;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Forms;

public class ClientFormData
{
    [Display(Name = "Client Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? ClientImage { get; set; }  // File upload for client image
    public ImageFormData? Image { get; set; }

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Required")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Location", Prompt = "Enter location")]
    [DataType(DataType.Text)]
    public string? Location { get; set; } // This will hold the combined address

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    // Address-related fields
    public UserAddressFormData? Address { get; set; }
}