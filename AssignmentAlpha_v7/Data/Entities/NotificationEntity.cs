using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationEntity
{
    [Key]
    [Column(TypeName = "varchar(36)")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(NotificationTargetGroup))]
    public int NotificationTargetGroupId { get; set; } = 1;
    public virtual NotificationTargetGroupEntity NotificationTargetGroup { get; set; } = null!;

    [ForeignKey(nameof(NotificationType))] 
    public int NotificationTypeId { get; set; }
    public virtual NotificationTypeEntity NotificationType { get; set; } = null!;
    
    [Column(TypeName = "varchar(150)")]
    public string Icon { get; set; } = null!;
    
    [Column(TypeName = "nvarchar(500)")]
    public string Message { get; set; } = null!;
    
    [Column(TypeName = "date")]
    public DateTime Created { get; set; } = DateTime.Now;
    
    public virtual ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];
}