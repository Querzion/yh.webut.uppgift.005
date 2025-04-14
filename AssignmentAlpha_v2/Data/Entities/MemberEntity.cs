using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class MemberEntity : IdentityUser
{
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(75)")]
    public string FirstName { get; set; } = null!;

    
    [ProtectedPersonalData] 
    [Column(TypeName = "nvarchar(75)")]
    public string LastName { get; set; } = null!;
    
    
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string? JobTitle { get; set; }
    
    
    [ProtectedPersonalData]
    public DateTime? DateOfBirth { get; set; }
    
    public virtual MemberAddressEntity? Address { get; set; }
    public virtual MemberImageEntity? MemberImage { get; set; }
}