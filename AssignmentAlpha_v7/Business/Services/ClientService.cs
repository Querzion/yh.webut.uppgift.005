using System.Diagnostics;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IClientService
{
    Task<ClientServiceResult> CreateClientAsync(AddClientFormData form);
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

            public async Task<ClientServiceResult> CreateClientAsync(AddClientFormData form)
            {
                if (form == null)
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 400,
                        Error = "Client form is required."
                    };
                }

                await _clientRepository.BeginTransactionAsync();

                try
                {
                    var existsResult = await _clientRepository.ExistsAsync(x => x.ClientName == form.ClientName);

                    if (existsResult.Succeeded && existsResult.Result)
                    {
                        await _clientRepository.RollbackTransactionAsync();
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 409,
                            Error = "A client with that name already exists."
                        };
                    }

                    var entity = form.MapTo<ClientEntity>();

                    if (form.Address != null)
                    {
                        var addressEntity = form.Address.MapTo<AddressEntity>();
                        entity.Address = addressEntity;
                    }

                    var createResult = await _clientRepository.AddAsync(entity);

                    if (!createResult.Succeeded || !createResult.Result)
                    {
                        await _clientRepository.RollbackTransactionAsync();
                        return new ClientServiceResult
                        {
                            Succeeded = false,
                            StatusCode = 500,
                            Error = "Failed to create client."
                        };
                    }

                    await _clientRepository.CommitTransactionAsync();
                    return new ClientServiceResult
                    {
                        Succeeded = true,
                        StatusCode = 201
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
                    Result = new[] { repoResult.Result }
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
                    Result = new[] { repoResult.Result }
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
                    Result = new[] { repoResult.Result }
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

                var entityResult = await _clientRepository.GetEntityAsync(x => x.Id == id);

                if (!entityResult.Succeeded || entityResult.Result == null)
                {
                    return new ClientServiceResult
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Error = "Client not found."
                    };
                }

                try
                {
                    var deleteResult = await _clientRepository.DeleteAsync(entityResult.Result);

                    return new ClientServiceResult
                    {
                        Succeeded = deleteResult.Succeeded && deleteResult.Result,
                        StatusCode = deleteResult.Succeeded
                            ? deleteResult.Result ? 200 : 500
                            : 500,
                        Error = deleteResult.Succeeded && deleteResult.Result ? null : "Failed to delete client."
                    };
                }
                catch (Exception ex)
                {
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