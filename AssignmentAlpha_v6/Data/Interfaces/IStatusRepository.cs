using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IStatusRepository : IBaseRepository<StatusEntity, Status>
{
    
}