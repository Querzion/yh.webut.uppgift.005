using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationTypeEntity
{
    [Key]
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(75)")]
    public string NotificationType { get; set; } = null!;
    
    public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];
}