using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class StatusRepository(AppDbContext context) : BaseRepository<StatusEntity, Status>(context), IStatusRepository
{
    
}