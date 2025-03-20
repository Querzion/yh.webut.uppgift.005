using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class ApplicationUser : IdentityUser
{
    #region ChatGPT Memo
    
    public UserProfile? Profile { get; set; }
    
    #endregion
}

public class UserProfile
{
    #region ChatGPT Memo - In Parts

    [Key]
    [ForeignKey("ApplicationUser")]
    [PersonalData]
    public string UserId { get; set; } = null!;
        
    public ApplicationUser User { get; set; } = null!;

    #endregion
    
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; } = null!;

    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; } = null!;
}