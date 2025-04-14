using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

[Index(nameof(StatusName), IsUnique = true)]
public class StatusEntity
{
    [Key]
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(75)")]
    public string StatusName { get; set; } = null!;
    
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}