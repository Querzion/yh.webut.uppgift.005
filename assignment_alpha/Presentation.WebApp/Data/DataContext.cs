using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presentation.WebApp.Models;

namespace Presentation.WebApp.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserModel>(options)
{
    // public DbSet<ProjectEntity> Projects { get; set; }
}