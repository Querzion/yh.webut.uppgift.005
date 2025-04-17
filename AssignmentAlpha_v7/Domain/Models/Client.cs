namespace Domain.Models;

public class Client
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string ClientName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    
    public virtual Image? ClientImage { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime Date { get; set; } = DateTime.Now;
    public string? Location { get; set; }
    
    public virtual UserAddress? UserAddress { get; set; } = new UserAddress();
}