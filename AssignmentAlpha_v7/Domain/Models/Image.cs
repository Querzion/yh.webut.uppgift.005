using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Image
{
    [StringLength(36)]
    public string Id { get; set; } = null!;

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Alternative Text")]
    public string? AltText { get; set; }

    [Display(Name = "Uploaded At")]
    [DataType(DataType.DateTime)]
    public DateTime UploadedAt { get; set; }
    
    public Client? Client { get; set; }
    public Project? Project { get; set; }
    public User? User { get; set; }
}