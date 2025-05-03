using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IUserRepository : IBaseRepository<AppUser, User>
{
    Task<int> GetUserCountAsync();
}