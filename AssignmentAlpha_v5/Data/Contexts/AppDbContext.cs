using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    // LazyLoading is implemented in Program.cs and is no longer needed here.
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     base.OnConfiguring(optionsBuilder);
    //     optionsBuilder.UseLazyLoadingProxies();
    // }
    
    public virtual DbSet<NotificationEntity> Notifications { get; set; }
    public virtual DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }
    public virtual DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public virtual DbSet<NotificationTargetGroupEntity> NotificationTargetGroups { get; set; }
    public virtual DbSet<ClientEntity> Clients { get; set; }
    public virtual DbSet<StatusEntity> Statuses { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<ProjectMemberEntity> ProjectMembers { get; set; }
    public virtual DbSet<UserAddressEntity> Addresses { get; set; }
    public virtual DbSet<ImageEntity> ImagePaths { get; set; }
    public virtual DbSet<TagEntity> Tags { get; set; }
}