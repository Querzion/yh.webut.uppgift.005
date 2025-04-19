using System.Diagnostics;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IAddressService
{
    Task<AddressServiceResult> GetOrCreateAddressAsync(AddressFormData form);
}

public class AddressService(IAddressRepository addressRepository) : IAddressService
{
    private readonly IAddressRepository _addressRepository = addressRepository;
    
    #region Without TransactionManagement (In order to not partake in multiple transactions at MemberCreation)

        public async Task<AddressServiceResult> GetOrCreateAddressAsync(AddressFormData form)
        {
            if (form == null!)
                return new AddressServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };

            // Check if the address already exists in the database
            var existingAddress = await _addressRepository.GetAsync(a =>
                a.StreetName == form.StreetName &&
                a.PostalCode == form.PostalCode &&
                a.City == form.City &&
                (form.County == null || a.County == form.County) &&
                (form.Country == null || a.Country == form.Country)
            );

            if (existingAddress != null)
            {
                // If address exists, return it (don't create a new one)
                return new AddressServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200, // Address exists, no need to create a new one
                    Result = new List<Address> { existingAddress.MapTo<Address>() }
                };
            }

            try
            {
                // If address doesn't exist, create a new one
                var newAddress = new AddressEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    StreetName = form.StreetName,
                    PostalCode = form.PostalCode,
                    City = form.City,
                    County = string.IsNullOrEmpty(form.County) ? "Unknown" : form.County,
                    Country = string.IsNullOrEmpty(form.Country) ? "Unknown" : form.Country

                };

                var addResult = await _addressRepository.AddAsync(newAddress);

                if (addResult.Succeeded)
                {
                    return new AddressServiceResult
                    {
                        Succeeded = true,
                        StatusCode = 201, // Created
                        Result = new List<Address> { addResult.Result.MapTo<Address>() }
                    };
                }
                else
                {
                    return new AddressServiceResult { Succeeded = false, StatusCode = 500, Error = addResult.Error ?? "Failed to add address." };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new AddressServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
            }
        }

    #endregion

    #region With TransactionManagement (Faulty though, because it's being called multiple times when used inside of another service.)

        // public async Task<AddressServiceResult> GetOrCreateAddressAsync(AddressFormData form)
        // {
        //     if (form == null!)
        //         return new AddressServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };
        //     
        //     var existsResult = await _addressRepository.ExistsAsync(a =>
        //         a.StreetName == form.StreetName &&
        //         a.PostalCode == form.PostalCode &&
        //         a.City == form.City &&
        //         (form.County == null || a.County == form.County) &&
        //         (form.Country == null || a.Country == form.Country)
        //     );
        //
        //     if (existsResult.Succeeded)
        //         return new AddressServiceResult { Succeeded = false, StatusCode = 409, Error = "Address already exists." };
        //
        //     try
        //     {
        //         // Begin transaction
        //         await _addressRepository.BeginTransactionAsync();
        //
        //         // Map form data to AddressEntity
        //         var newAddress = new AddressEntity
        //         {
        //             Id = Guid.NewGuid().ToString(),
        //             StreetName = form.StreetName,
        //             PostalCode = form.PostalCode,
        //             City = form.City,
        //             County = form.County ?? "Värmland",
        //             Country = form.Country ?? "Sweden"
        //         };
        //
        //         // Add the new address to the repository
        //         var addResult = await _addressRepository.AddAsync(newAddress);
        //
        //         if (addResult.Succeeded)
        //         {
        //             // Commit transaction
        //             await _addressRepository.CommitTransactionAsync();
        //
        //             // Return success result with the newly created address
        //             return new AddressServiceResult
        //             {
        //                 Succeeded = true,
        //                 StatusCode = 201,
        //                 Result = new List<Address> { addResult.Result.MapTo<Address>() }
        //             };
        //         }
        //         else
        //         {
        //             // Rollback transaction on failure
        //             await _addressRepository.RollbackTransactionAsync();
        //             return new AddressServiceResult { Succeeded = false, StatusCode = 500, Error = addResult.Error ?? "Failed to add address." };
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // Rollback transaction on error
        //         await _addressRepository.RollbackTransactionAsync();
        //         Debug.WriteLine(ex.Message);
        //         return new AddressServiceResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        //     }
        // }

    #endregion
}