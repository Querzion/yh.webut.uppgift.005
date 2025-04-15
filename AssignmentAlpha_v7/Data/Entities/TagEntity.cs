using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

[Index(nameof(TagName), IsUnique = true)]
public class TagEntity
{
    [Key]
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(75)")]
    public string TagName { get; set; } = null!;
}