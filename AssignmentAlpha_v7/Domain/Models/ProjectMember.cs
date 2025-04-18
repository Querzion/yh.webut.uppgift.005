using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class ProjectMember
{
    public int Id { get; set; }

    [StringLength(36)]
    public string ProjectId { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    
    [StringLength(36)]
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}