using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;

namespace Data.Repositories;

public class AddressRepository(AppDbContext context) : BaseRepository<AddressEntity, Address>(context), IAddressRepository
{
    public virtual async Task<RepositoryResult<bool>> AddAddressAndLinkToUserAsync(AddressEntity newAddress, User user)
    {
        // Check if the address already exists in the database
        var existingAddressResult = await FindEntityAsync(a => a.StreetName == newAddress.StreetName && a.PostalCode == newAddress.PostalCode && a.City == newAddress.City);

        if (existingAddressResult.Succeeded && existingAddressResult.Result != null)
        {
            // If the address already exists, link the user to the existing address
            user.AddressId = existingAddressResult.Result.Id;
        }
        else
        {
            // Address doesn't exist, so create a new one and link it to the user
            _context.Add(newAddress);
            await _context.SaveChangesAsync();
            user.AddressId = newAddress.Id;
        }

        // Save the user after linking the address
        await _context.SaveChangesAsync();
        return new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
    }
}