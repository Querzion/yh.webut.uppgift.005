using Business.Models;
using Data.Interfaces;
using Domain.Extensions;

namespace Business.Services;

public interface IClientService
{
    Task<ClientServiceResult> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    #region CRUD

        #region Read

            public async Task<ClientServiceResult> GetClientsAsync()
            {
                var result = await _clientRepository.GetAllAsync();
                return result.MapTo<ClientServiceResult>();
            }

        #endregion

    #endregion
    
    
}