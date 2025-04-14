using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class UserAddressRepository(AppDbContext context) : BaseRepository<UserAddressEntity, UserAddress>(context), IUserAddressRepository
{
    
}