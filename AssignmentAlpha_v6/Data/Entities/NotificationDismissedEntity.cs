using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationDismissedEntity
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    [Column(TypeName = "varchar(36)")]
    public string UserId { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;
    
    [ForeignKey(nameof(Notification))]
    [Column(TypeName = "varchar(36)")]
    public string NotificationId { get; set; } = null!;
    public virtual NotificationEntity Notification { get; set; } = null!;
}