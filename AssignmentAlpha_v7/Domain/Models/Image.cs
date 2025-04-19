using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Image
{
    public string Id { get; set; } = null!;

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Alternative Text")]
    public string? AltText { get; set; }

    [Display(Name = "Uploaded At")]
    [DataType(DataType.DateTime)]
    public DateTime UploadedAt { get; set; }
    
    public virtual Client? Client { get; set; }
    public virtual Project? Project { get; set; }
    public virtual User? User { get; set; }
}