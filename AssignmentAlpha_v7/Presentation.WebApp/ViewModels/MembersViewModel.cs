using Domain.Models;
using Presentation.WebApp.ViewModels.Adds;
using Presentation.WebApp.ViewModels.Authentications;
using Presentation.WebApp.ViewModels.Edits;

namespace Presentation.WebApp.ViewModels;

#region ChatGPT Helped with this part.

    public class MembersViewModel
    {
        public string Title { get; set; } = null!;

        public IEnumerable<User>? Members { get; set; } = [];

        public SignUpViewModel RegistrationForm { get; set; } = new();
        
        public AddMemberViewModel AddMember { get; set; } = new();
        
        public EditMemberViewModel EditMember { get; set; } = new();
        
        public SignInViewModel Login { get; set; } = new();
    }

#endregion

