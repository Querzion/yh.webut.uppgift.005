using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IProjectRepository : IBaseRepository<ProjectEntity, Project>
{
    
}