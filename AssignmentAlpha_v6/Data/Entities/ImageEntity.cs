using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class ImageEntity
{
    [Key]
    [Column(TypeName = "varchar(36)")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column(TypeName = "nvarchar(200)")]
    public string? ImageUrl { get; set; }

    // Optional metadata
    [Column(TypeName = "nvarchar(200)")]
    public string? AltText { get; set; }

    [Column(TypeName = "date")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<ClientEntity>? Clients { get; set; }
    public virtual ICollection<ProjectEntity>? Projects { get; set; }
    public virtual ICollection<AppUser>? Users { get; set; }
}