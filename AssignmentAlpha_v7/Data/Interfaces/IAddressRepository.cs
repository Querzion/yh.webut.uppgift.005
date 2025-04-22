using Data.Entities;
using Data.Models;
using Domain.Models;

namespace Data.Interfaces;

public interface IAddressRepository : IBaseRepository<AddressEntity, Address>
{
    Task<RepositoryResult<bool>> AddAddressAndLinkToUserAsync(AddressEntity newAddress, User user);
}