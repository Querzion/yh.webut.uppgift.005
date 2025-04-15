namespace Domain.Models;

public class Client
{
    public string Id { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Location { get; set; }
    public string? PhoneNumber { get; set; }
    
    // Optional: image path or base64 string
    public string? ImagePath { get; set; }
}