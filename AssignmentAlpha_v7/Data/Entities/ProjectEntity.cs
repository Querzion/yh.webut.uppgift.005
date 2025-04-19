using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Data.Entities;

public class ProjectEntity
{
    [Key, Column(TypeName = "varchar(36)")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Column(TypeName = "nvarchar(150)")]
    public string ProjectName { get; set; } = null!;

    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }
    
    // Strict ONE TO ONE RELATIONSHIP
    [ForeignKey(nameof(Image)), Column(TypeName = "varchar(36)")]
    public string? ImageId { get; set; }
    public virtual ImageEntity? Image { get; set; }

    [ForeignKey(nameof(Client)), Column(TypeName = "varchar(36)")]
    public string ClientId { get; set; } = null!;
    public virtual ClientEntity Client { get; set; } = null!;

    [ForeignKey(nameof(User)), Column(TypeName = "varchar(36)")]
    public string UserId { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;

    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }
    public virtual StatusEntity Status { get; set; } = null!;
    
    public virtual ICollection<ProjectMemberEntity> ProjectMembers { get; set; } = [];
}