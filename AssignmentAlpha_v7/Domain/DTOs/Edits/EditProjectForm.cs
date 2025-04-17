using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Forms;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Edits;

public class EditProjectForm : ProjectFormData
{
    [Required(ErrorMessage = "Project ID is required.")]
    [Display(Name = "Project ID")]
    public string Id { get; set; } = null!;
}