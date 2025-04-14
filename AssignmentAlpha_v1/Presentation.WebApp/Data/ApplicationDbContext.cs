using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presentation.WebApp.Models;

namespace Presentation.WebApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<UserProfile> UserProfiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region ChatGPT Generated Code
        
        builder.Entity<UserProfile>()
            .HasOne(up => up.User) // A UserProfile has one ApplicationUser
            .WithOne(u => u.Profile) // An ApplicationUser has one UserProfile
            .HasForeignKey<UserProfile>(up => up.UserId) // UserId is the foreign key
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete from UserProfile to ApplicationUser
         
        #endregion
        
        base.OnModelCreating(builder);
    }
}