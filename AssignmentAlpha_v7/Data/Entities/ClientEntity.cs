using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

[Index(nameof(ClientName), IsUnique = true)]
public class ClientEntity
{
    [Key, Column(TypeName = "varchar(36)")]
    public string Id { get; set; } = null!;

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Required")]
    [Column(TypeName = "nvarchar(200)")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email")]
    [Column(TypeName = "nvarchar(200)")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    [Column(TypeName = "varchar(20)")]
    public string? PhoneNumber { get; set; }

    // Strict ONE TO ONE RELATIONSHIP
    [ForeignKey(nameof(Image)), Column(TypeName = "varchar(36)")]
    public string? ImageId { get; set; }
    public virtual ImageEntity? Image { get; set; }

    [Column(TypeName = "bit")] 
    public bool IsActive { get; set; } = true;

    [Column(TypeName = "datetime")] 
    public DateTime Date { get; set; } = DateTime.Now;

    [Column(TypeName = "varchar(36)")]
    public string? AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual AddressEntity? Address { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}