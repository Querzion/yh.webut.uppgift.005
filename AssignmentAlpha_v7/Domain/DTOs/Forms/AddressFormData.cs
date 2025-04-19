using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Forms;

public class AddressFormData
{
    [Display(Name = "Street Name", Prompt = "Enter street name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Street name is required")]
    public string StreetName { get; set; } = null!;

    [Display(Name = "Postal Code", Prompt = "Enter postal code")]
    [DataType(DataType.PostalCode)]
    [Required(ErrorMessage = "Postal code is required")]
    public string PostalCode { get; set; } = null!;

    [Display(Name = "City", Prompt = "Enter city")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = null!;

    [Display(Name = "County", Prompt = "Enter county (optional)")]
    [DataType(DataType.Text)]
    public string? County { get; set; }

    [Display(Name = "Country", Prompt = "Enter country (optional)")]
    [DataType(DataType.Text)]
    public string? Country { get; set; }
}