using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Address
{
    [StringLength(36)]
    public string Id { get; set; } = null!;

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? Country { get; set; }
}