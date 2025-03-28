

using Business.Factories;
using Business.Interfaces;
using Data.Entities;
using Domain.DTOs;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.DTOs.Registrations;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

#region Second Version

    public class MemberService(UserManager<MemberEntity> userManager) : IMemberService
    {
        private readonly UserManager<MemberEntity> _userManager = userManager;
        
        public async Task<IEnumerable<Member>> GetMembersAsync()
        {
            var list = await _userManager.Users.ToListAsync();
            var members = list.Select(MemberFactory.ToModel);

            return members;
        }

        // public async Task<bool> CreateMemberAsync(MemberSignUpForm registrationForm)
        // {
        //     var memberEntity = MemberFactory.ToEntity(registrationForm);
        //     var result = await _userManager.CreateAsync(memberEntity);
        //     return result.Succeeded;
        // }

        public async Task<bool> AddMemberAsync(AddMemberForm addMemberForm)
        {
            var memberEntity = MemberFactory.ToEntity(addMemberForm);
            var result = await _userManager.CreateAsync(memberEntity, "BytMig123!");
            return result.Succeeded;
        }

        public Task<bool> UpdateMemberAsync(EditMemberForm updateForm)
        {
            throw new NotImplementedException();
        }
    }

#endregion

#region First Version

    // public class MemberService(UserManager<MemberEntity> userManager) : IMemberService
    // {
    //     private readonly UserManager<MemberEntity> _userManager = userManager;
    //
    //     public async Task<IEnumerable<Member>> GetAllMembersAsync()
    //     {
    //         var list = await _userManager.Users.ToListAsync();
    //         
    //         var members = list.Select(member => new Member
    //         {
    //             Id = member.Id,
    //             FirstName = member.FirstName,
    //             LastName = member.LastName,
    //             Email = member.Email,
    //             PhoneNumber = member.PhoneNumber,
    //             JobTitle = member.JobTitle,
    //             // Address = 
    //         });
    //
    //         return members;
    //     }
    // }

#endregion