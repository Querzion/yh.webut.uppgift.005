using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class UserAddressEntity
{
    [Key, ForeignKey("User")]
    [Column(TypeName = "varchar(36)")]
    public string UserId { get; set; } = null!;
    
    [Column(TypeName = "nvarchar(150)")]
    public string? StreetName { get; set; }
    
    [Column(TypeName = "nvarchar(8)")]
    public string? PostalCode { get; set; }
    
    [Column(TypeName = "nvarchar(150)")]
    public string? City { get; set; }
    
    public virtual AppUser User { get; set; } = null!;
}