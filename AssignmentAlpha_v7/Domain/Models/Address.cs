namespace Domain.Models;

public class Address
{
    public string Id { get; set; } = null!;

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? Country { get; set; }
}