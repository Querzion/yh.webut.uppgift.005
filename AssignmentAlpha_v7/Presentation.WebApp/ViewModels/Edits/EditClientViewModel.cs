using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Adds;
using Presentation.WebApp.ViewModels.Forms;

namespace Presentation.WebApp.ViewModels.Edits;

public class EditClientViewModel : ClientFormViewModel
{
    [Required(ErrorMessage = "Required")]
    public string Id { get; set; } = null!;
}