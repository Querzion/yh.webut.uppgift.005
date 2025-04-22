using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

[Index(nameof(StreetName), nameof(PostalCode), nameof(City), IsUnique = true)]
public class AddressEntity
{
    [Key]
    [Column(TypeName = "varchar(36)")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column(TypeName = "nvarchar(150)")]
    public string? StreetName { get; set; }

    [Column(TypeName = "nvarchar(8)")]
    public string? PostalCode { get; set; }

    [Column(TypeName = "nvarchar(150)")]
    public string? City { get; set; }

    // [Column(TypeName = "nvarchar(150)")]
    // public string? County { get; set; }
    //
    // [Column(TypeName = "nvarchar(150)")]
    // public string? Country { get; set; }

    // Optional navigation properties (1:1 or 1:many depending on design)
    public virtual ICollection<AppUser> Users { get; set; } = [];
    public virtual ICollection<ClientEntity> Clients { get; set; } = [];
}