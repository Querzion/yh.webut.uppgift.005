namespace Domain.Models;

public class UserAddress
{
    public string UserId { get; set; } = null!;
    
    public string? StreetName { get; set; }
    
    public string? PostalCode { get; set; }
    
    public string? City { get; set; }
    
    public virtual User User { get; set; } = null!;
}