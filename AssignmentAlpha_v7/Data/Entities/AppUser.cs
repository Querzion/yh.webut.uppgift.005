using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class AppUser : IdentityUser
{
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(75)")]
    public string? FirstName { get; set; }

    [ProtectedPersonalData] 
    [Column(TypeName = "nvarchar(75)")]
    public string? LastName { get; set; }

    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string? JobTitle { get; set; }

    [ProtectedPersonalData]
    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [Column(TypeName = "varchar(36)")]
    public string? ImageId { get; set; }
    [ForeignKey(nameof(ImageId))]
    public virtual ImageEntity? Image { get; set; }
    
    [Column(TypeName = "varchar(36)")]
    public string? AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual AddressEntity? Address { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
    public virtual ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];
}