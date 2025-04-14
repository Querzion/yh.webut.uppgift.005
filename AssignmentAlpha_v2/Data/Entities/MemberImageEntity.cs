using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class MemberImageEntity
{
    [Key, ForeignKey("Member")]
    [Column(TypeName = "varchar(36)")]
    public string MemberId { get; set; } = null!;
    
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(200)")]
    public string? MemberImagePath { get; set; }
    
    public virtual MemberEntity Member { get; set; } = null!;
}