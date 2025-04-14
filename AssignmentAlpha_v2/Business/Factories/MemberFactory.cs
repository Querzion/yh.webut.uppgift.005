using Data.Entities;
using Domain.DTOs;
using Domain.DTOs.Adds;
using Domain.Models;

namespace Business.Factories;

public class MemberFactory
{
    // From DTO to Entity 
    public static MemberEntity ToEntity(MemberSignUpForm form)
    {
        return form == null
            ? new MemberEntity()
            : new MemberEntity
            {
                UserName = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email
            };
    }
    
    public static MemberEntity ToEntity(AddMemberForm form)
    {
        return form == null
            ? new MemberEntity()
            : new MemberEntity
            {
                UserName = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                JobTitle = form.JobTitle,
                PhoneNumber = form.PhoneNumber,
                Address = new MemberAddressEntity
                {
                    StreetName = form.StreetName,
                    PostalCode = form.PostalCode,
                    City = form.City
                },
                DateOfBirth = form.DateOfBirth,
                MemberImage = new MemberImageEntity
                {
                    MemberImagePath = form.MemberImagePath
                } 
            };
    }
    
    // From Entity to Model
    public static Member ToModel(MemberEntity user)
    {
        return user == null
            ? new Member()
            : new Member
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                JobTitle = user.JobTitle,
                Address = user.Address == null 
                    ? new MemberAddress() // Default to empty Address if null
                    : new MemberAddress
                    {
                        StreetName = user.Address.StreetName,
                        PostalCode = user.Address.PostalCode,
                        City = user.Address.City
                    },
                MemberImage = user.MemberImage == null 
                    ? new MemberImage() // Default to empty MemberImage if null
                    : new MemberImage
                    {
                        MemberImagePath = user.MemberImage.MemberImagePath
                    }
            };
    }
}