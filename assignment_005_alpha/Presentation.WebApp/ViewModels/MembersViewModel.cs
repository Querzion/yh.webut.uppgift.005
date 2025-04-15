using Domain.Models;

namespace Presentation.WebApp.ViewModels;

#region ChatGPT Helped with this part.

    public class MembersViewModel
    {
        public IEnumerable<Member>? Members { get; set; }
        public AddMemberForm AddMemberForm { get; set; } = new AddMemberForm();
        public EditMemberForm EditMemberForm { get; set; } = new EditMemberForm();
    }

#endregion

