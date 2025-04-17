namespace Domain.Models;

public class Project
{
    public string Id { get; set; } = null!;
    
    public string? Image { get; set; }
    
    public string ProjectName { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public decimal? Budget { get; set; }
    
    public virtual Image? ProjectImage { get; set; }
    
    public virtual Client Client { get; set; } = null!;
    
    public virtual User User { get; set; } = null!;
    
    public virtual Status Status { get; set; } = null!;
    
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = [];
}