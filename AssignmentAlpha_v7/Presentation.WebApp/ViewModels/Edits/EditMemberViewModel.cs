using System.ComponentModel.DataAnnotations;
using Presentation.WebApp.ViewModels.Forms;

namespace Presentation.WebApp.ViewModels.Edits;

public class EditMemberViewModel : MemberFormViewModel
{
    [Required(ErrorMessage = "Required")]
    public string Id { get; set; } = null!;
}