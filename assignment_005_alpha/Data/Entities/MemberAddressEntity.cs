using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class MemberAddressEntity
{
    [Key, ForeignKey("Member")]
    [Column(TypeName = "varchar(36)")]
    public string MemberId { get; set; } = null!;
    
    [Column(TypeName = "nvarchar(150)")]
    public string? StreetName { get; set; }
    
    [Column(TypeName = "nvarchar(8)")]
    public string? PostalCode { get; set; }
    
    [Column(TypeName = "nvarchar(150)")]
    public string? City { get; set; }
    
    public virtual MemberEntity Member { get; set; } = null!;
}