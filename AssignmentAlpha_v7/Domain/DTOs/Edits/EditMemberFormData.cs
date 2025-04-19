using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Adds;
using Domain.DTOs.Forms;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Edits;

public class EditMemberFormData : MemberFormData
{
    [Required(ErrorMessage = "Member ID is required.")]
    [Display(Name = "Member ID")]
    [StringLength(36)]
    public string Id { get; set; } = null!;
}