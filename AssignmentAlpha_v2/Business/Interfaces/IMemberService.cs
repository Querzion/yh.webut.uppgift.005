using Domain.DTOs;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Registrations;
using Domain.Models;

namespace Business.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetMembersAsync();
    // Task<bool> CreateMemberAsync(MemberSignUpForm registrationForm);
    Task<bool> AddMemberAsync(AddMemberForm addMemberForm);
    Task<bool> UpdateMemberAsync(EditMemberForm updateForm);
}