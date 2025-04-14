using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IUserAddressRepository : IBaseRepository<UserAddressEntity, UserAddress>
{
    
}