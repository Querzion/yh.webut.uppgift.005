using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationTargetGroupEntity
{
    [Key]
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(75)")]
    public string TargetGroup { get; set; } = null!;
    
    public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];
}