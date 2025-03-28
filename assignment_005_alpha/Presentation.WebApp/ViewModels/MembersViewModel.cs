using Domain.Models;
using Presentation.WebApp.ViewModels.Edits;
using Presentation.WebApp.ViewModels.Logins;
using Presentation.WebApp.ViewModels.Registrations;
using Presentation.WebApp.ViewModels.SignUps;

namespace Presentation.WebApp.ViewModels;

#region ChatGPT Helped with this part.

    public class MembersViewModel
    {
        public string Title { get; set; } = null!;

        public IEnumerable<Member>? Members { get; set; } = [];

        public MemberSignUpViewModel RegistrationForm { get; set; } = new();
        
        public AddMemberViewModel AddMember { get; set; } = new();
        
        public EditMemberViewModel EditMember { get; set; } = new();
        
        public MemberLoginViewModel Login { get; set; } = new();
    }

#endregion

