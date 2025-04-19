using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Forms;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Edits;

public class EditClientFormData : ClientFormData
{
    [Required(ErrorMessage = "Client ID is required.")]
    [Display(Name = "Client ID")]
    [StringLength(36)]
    public string Id { get; set; } = null!;
}