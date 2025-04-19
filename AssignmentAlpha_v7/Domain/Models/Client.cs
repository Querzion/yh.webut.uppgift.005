using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Client
{
    public string Id { get; set; } = null!;

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Profile Image")]
    public string? ImageId { get; set; }
    public virtual Image? Image { get; set; }

    [Display(Name = "Address")]
    public string? AddressId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime Date { get; set; } = DateTime.Now;

    public virtual ICollection<Project> Projects { get; set; } = [];
    
    public Address? Address { get; set; }
}