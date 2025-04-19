using Domain.Models;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Authentications;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.ListItems;

namespace Presentation.WebApp.ViewModels;

#region ChatGPT Helped with this part.

    public class MembersViewModel
    {
        public string Title { get; set; } = null!;

        public IEnumerable<MemberListItemViewModel>? Members { get; set; } = [];
        
        public AddMemberViewModel AddMember { get; set; } = new();
        
        public EditMemberViewModel EditMember { get; set; } = new();
        
        public SignInViewModel Login { get; set; } = new();
       
        public SignUpViewModel RegistrationForm { get; set; } = new();
    }

#endregion

