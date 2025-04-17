using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.WebApp.ViewModels.Forms;

namespace Presentation.WebApp.ViewModels.Edits;

public class EditProjectViewModel : ProjectFormViewModel
{
    [Required(ErrorMessage = "Required")]
    public string Id { get; set; } = null!;
}