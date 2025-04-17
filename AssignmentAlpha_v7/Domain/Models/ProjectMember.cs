namespace Domain.Models;

public class ProjectMember
{
    public int Id { get; set; }

    public string ProjectId { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}