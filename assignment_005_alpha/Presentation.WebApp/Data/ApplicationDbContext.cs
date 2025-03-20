using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Presentation.WebApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    
}