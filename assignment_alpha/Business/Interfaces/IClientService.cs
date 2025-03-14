using Business.DTOs;

namespace Business.Interfaces;

public interface IClientService
{
    Task<IResult> CreateClientAsync(ClientRegistrationForm registrationForm);
    Task<IResult> GetAllClientsAsync();
    Task<IResult> GetClientByIdAsync(int id);
    Task<IResult> GetClientByNameAsync(string clientName);
    Task<IResult> UpdateClientAsync(int id, ClientUpdateForm updateForm);
    Task<IResult> DeleteClientAsync(int id);
    Task<IResult> CheckIfClientExists(string customerName);
}