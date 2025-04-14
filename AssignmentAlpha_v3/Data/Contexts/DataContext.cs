using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext : IdentityDbContext<MemberEntity>
{
    
}