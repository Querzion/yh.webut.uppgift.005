using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IClientRepository : IBaseRepository<ClientEntity, Client>
{
    
}