using System.Diagnostics;
using Business.Factories;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.Extensions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IClientService
{
    Task<ClientServiceResult> AddClientAsync(AddClientFormData form);
    Task<ClientServiceResult> GetClientsAsync();
    Task<ClientServiceResult> GetAllClientsAsync();
    Task<ClientServiceResult> GetClientByIdAsync(string id);
    Task<ClientServiceResult> GetClientByNameAsync(string clientName);
    Task<ClientServiceResult> GetClientByEmailAsync(string email);
    Task<ClientServiceResult> UpdateClientAsync(string id, EditClientFormData update);
    Task<ClientServiceResult> DeleteClientAsync(string id);
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    #region CRUD (Mainly forged by spanking ChatGPT. . .)

        #region Create

            public async Task<ClientServiceResult> AddClientAsync(AddClientFormData formData)
            {
                if (formData == null)
                    return new ClientServiceResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };

                // Search for existing client by ClientName (case-insensitive using LIKE)
                var existsResult = await _clientRepository.ExistsAsync(c => EF.Functions.Like(c.ClientName, formData.ClientName));
                if (existsResult.Succeeded)
                    return new ClientServiceResult { Succeeded = false, StatusCode = 409, Error = "Client name already exists." };

                try
                {
                    await _clientRepository.BeginTransactionAsync();

                    var clientEntity = ClientFactory.CreateFromAddClientForm(formData);

                    var addResult = await _clientRepository.AddAsync(clientEntity);
                    if (!addResult.Succeeded)
                    {
                        await _clientRepository.RollbackTransactionAsync(); // Rollback transaction if adding client fails
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Error = "Unable to add client."
                        };
                    }

                    await _clientRepository.CommitTransactionAsync();

                    return new ClientServiceResult { Succeeded = true, StatusCode = 201 };
                }
                catch (Exception ex)
                {
                    await _clientRepository.RollbackTransactionAsync();
                 
                    Debug.WriteLine(ex.Message);
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = ex.Message
                    };
                }
            }

        #endregion
    
    
        #region Read

            public async Task<ClientServiceResult> GetAllClientsAsync()
            {
                var result = await _clientRepository.GetAllAsync();
                return result.MapTo<ClientServiceResult>();
            }
            
            public async Task<ClientServiceResult> GetClientsAsync()
            {
                var repoResult = await _clientRepository.GetAllAsync();
            
                return new ClientServiceResult
                {
                    Succeeded = repoResult.Succeeded,
                    StatusCode = repoResult.Succeeded ? 200 : 500,
                    Error = repoResult.Error,
                    Result = repoResult.Result
                };
            }
            
            public async Task<ClientServiceResult> GetClientByIdAsync(string id)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Error = "Client ID is required."
                    };
                }

                var repoResult = await _clientRepository.GetAsync(x => x.Id == id);

                if (!repoResult.Succeeded || repoResult.Result == null)
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Error = "Client not found."
                    };
                }

                return new ClientServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = [repoResult.Result]
                };
            }

            public async Task<ClientServiceResult> GetClientByNameAsync(string clientName)
            {
                if (string.IsNullOrWhiteSpace(clientName))
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Error = "Client name is required."
                    };
                }

                var repoResult = await _clientRepository.GetAsync(x => x.ClientName == clientName);

                if (!repoResult.Succeeded || repoResult.Result == null)
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Error = "Client not found."
                    };
                }

                return new ClientServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = [repoResult.Result]
                };
            }

            
            public async Task<ClientServiceResult> GetClientByEmailAsync(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Error = "Client email is required."
                    };
                }

                var repoResult = await _clientRepository.GetAsync(x => x.Email == email);

                if (!repoResult.Succeeded || repoResult.Result == null)
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Error = "Client not found."
                    };
                }

                return new ClientServiceResult
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = [repoResult.Result]
                };
            }

        #endregion

        #region Update

            public async Task<ClientServiceResult> UpdateClientAsync(string id, EditClientFormData update)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Error = "Client ID is required."
                    };
                }

                await _clientRepository.BeginTransactionAsync();

                try
                {
                    var entityResult = await _clientRepository.GetEntityAsync(x => x.Id == id);

                    if (!entityResult.Succeeded || entityResult.Result == null)
                    {
                        await _clientRepository.RollbackTransactionAsync();
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 404,
                            Error = "Client not found."
                        };
                    }

                    var entity = entityResult.Result;
                    entity = update.MapTo(entity);

                    var updateResult = await _clientRepository.UpdateAsync(entity);

                    if (!updateResult.Succeeded || !updateResult.Result)
                    {
                        await _clientRepository.RollbackTransactionAsync();
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Error = "Failed to update client."
                        };
                    }

                    await _clientRepository.CommitTransactionAsync();
                    return new ClientServiceResult
                    {
                        Succeeded = true,
                        StatusCode = 200
                    };
                }
                catch (Exception ex)
                {
                    await _clientRepository.RollbackTransactionAsync();
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = ex.Message
                    };
                }
            }

        #endregion
        
        #region Delete
        
            public async Task<ClientServiceResult> DeleteClientAsync(string id)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Error = "Client ID is required."
                    };
                }

                // Start a transaction
                await _clientRepository.BeginTransactionAsync();

                try
                {
                    // Get the client entity to be deleted
                    var entityResult = await _clientRepository.GetEntityAsync(x => x.Id == id);

                    if (!entityResult.Succeeded || entityResult.Result == null)
                    {
                        await _clientRepository.RollbackTransactionAsync();  // Rollback transaction if client not found
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 404,
                            Error = "Client not found."
                        };
                    }

                    // Call DeleteAsync from the repository to delete the client
                    var deleteResult = await _clientRepository.DeleteAsync(entityResult.Result);

                    if (!deleteResult.Succeeded)
                    {
                        await _clientRepository.RollbackTransactionAsync();  // Rollback transaction if delete fails
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Error = "Failed to delete client."
                        };
                    }

                    // Commit the transaction if delete is successful
                    await _clientRepository.CommitTransactionAsync();

                    return new ClientServiceResult
                    {
                        Succeeded = true,
                        StatusCode = 200
                    };
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of an exception
                    await _clientRepository.RollbackTransactionAsync();
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = ex.Message
                    };
                }
            }

        #endregion
    #endregion
    
    
}