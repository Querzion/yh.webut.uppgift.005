using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.WebApp.ViewModels.Forms;

namespace Presentation.WebApp.ViewModels.Adds;

public class AddProjectViewModel : ProjectFormViewModel
{
    public List<SelectListItem> ClientOptions { get; set; } = new List<SelectListItem>();
}