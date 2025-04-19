using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class AddressRepository(AppDbContext context) : BaseRepository<AddressEntity, Address>(context), IAddressRepository
{
    
}