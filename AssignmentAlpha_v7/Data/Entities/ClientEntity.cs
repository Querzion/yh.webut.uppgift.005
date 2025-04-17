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

    // [Display(Name = "Location", Prompt = "Enter location")]
    // [DataType(DataType.Text)]
    // [Column(TypeName = "nvarchar(200)")]
    // public string? Location { get; set; }

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    [Column(TypeName = "varchar(20)")]
    public string? PhoneNumber { get; set; }

    [ForeignKey(nameof(Image)), Column(TypeName = "varchar(36)")]
    public string? ImageId { get; set; }
    public virtual ImageEntity? Image { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();

    // New properties
    [Column(TypeName = "bit")]
    public bool IsActive { get; set; } = true;  // Default to active
        
    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; } = DateTime.Now;  // Date the client was created or updated
    
    [ForeignKey(nameof(UserAddress)), Column(TypeName = "varchar(36)")]
    public string? UserAddressId { get; set; }
    
    public virtual UserAddressEntity? UserAddress { get; set; }
    
    [NotMapped]
    public string? Location
    {
        get
        {
            // Combine address fields into a single location string.
            if (UserAddress != null)
            {
                return $"{UserAddress.StreetName}, {UserAddress.PostalCode}, {UserAddress.City}";
            }
            return null;
        }
    }
}