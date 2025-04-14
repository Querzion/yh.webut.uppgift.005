using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class MemberEntity : IdentityUser
{
    [ProtectedPersonalData]
    public string? FirstName { get; set; }
    
    
    [ProtectedPersonalData]
    public string? LastName { get; set; }
    
    
    [ProtectedPersonalData]
    public string? JobTitle { get; set; }
}