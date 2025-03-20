using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models;

public class UserRegistrationViewModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
}