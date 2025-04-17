using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<AppUser, User>(context), IUserRepository
{
    
}