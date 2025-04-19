using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IAddressRepository : IBaseRepository<AddressEntity, Address>
{
    
}