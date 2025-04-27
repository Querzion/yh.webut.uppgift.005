using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Forms;

public class ImageFormData
{
    [Display(Name = "Image URL", Prompt = "Provide an image URL")]
    [DataType(DataType.ImageUrl)]
    public string? ImageUrl { get; set; }

    [Display(Name = "Alt Text", Prompt = "Image description")]
    [DataType(DataType.Text)]
    public string? AltText { get; set; }
}