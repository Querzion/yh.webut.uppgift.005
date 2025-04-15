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

    // If each image is only related to one client, project, or user
    public string? ClientId { get; set; }
    public string? ProjectId { get; set; }
    public string? UserId { get; set; }
}