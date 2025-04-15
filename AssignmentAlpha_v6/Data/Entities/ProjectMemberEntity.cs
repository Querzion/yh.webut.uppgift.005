using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectMemberEntity
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Project))]
    [Column(TypeName = "varchar(36)")]
    public string ProjectId { get; set; } = null!;
    public virtual ProjectEntity Project { get; set; } = null!;
    
    [ForeignKey(nameof(User))]
    [Column(TypeName = "varchar(36)")]
    public string UserId { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;
}