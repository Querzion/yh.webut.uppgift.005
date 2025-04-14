using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity, Project>(context), IProjectRepository
{
    
}