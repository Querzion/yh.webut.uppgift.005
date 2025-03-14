using Business.DTOs;
using Business.Interfaces;

namespace Business.Services;

public class ClientService : IClientService
{
    public Task<IResult> CreateClientAsync(ClientRegistrationForm registrationForm)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetAllClientsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetClientByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetClientByNameAsync(string clientName)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateClientAsync(int id, ClientUpdateForm updateForm)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> DeleteClientAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> CheckIfClientExists(string customerName)
    {
        throw new NotImplementedException();
    }
}