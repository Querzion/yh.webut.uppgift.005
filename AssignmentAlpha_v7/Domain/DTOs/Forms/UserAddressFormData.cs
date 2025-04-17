using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Forms;

public class UserAddressFormData
{
    [Display(Name = "Street Name", Prompt = "Your Street Name")]
    [DataType(DataType.Text)]
    public string? StreetName { get; set; }

    [Display(Name = "Postal Code", Prompt = "Your Postal Code")]
    [DataType(DataType.Text)]
    public string? PostalCode { get; set; }

    [Display(Name = "City", Prompt = "Your City")]
    [DataType(DataType.Text)]
    public string? City { get; set; }
}