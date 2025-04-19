using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Forms;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Edits;

public class EditProjectFormData : ProjectFormData
{
    [Required(ErrorMessage = "Project ID is required.")]
    [Display(Name = "Project ID")]
    [StringLength(36)]
    public string Id { get; set; } = null!;
}