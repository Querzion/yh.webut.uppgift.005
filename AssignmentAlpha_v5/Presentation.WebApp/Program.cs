using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseLazyLoadingProxies());

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/signin";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/auth/signin";
}).AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admins", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Managers", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("Authenticated", policy => policy.RequireRole("Admin", "Manager", "User"));
});

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Comment/UnComment if it's to be used or not.
#region IdentityUser - RoleCreation

// This creates roles if they do not exist, BUT, they do this check every time the application is run,
// so one thing that one can do is to create the manual input functionality, and through that just have to use it once,
// OR you can just start it once, and then comment out this whole section to be honest.


    using (var scope = app.Services.CreateScope())
    {
        #region Create Roles

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = ["Admin", "Manager", "User"];

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

        #endregion
        
        #region Create Administrator Account

            var userManagerAdmin = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var adminUser = new AppUser { FirstName = "System", LastName = "Administrator", UserName = "admin@domain.com", Email = "admin@domain.com" };

            var adminExists = await userManagerAdmin.Users.AnyAsync(u => u.Email == adminUser.Email);
            if (!adminExists)
            {
                var result = await userManagerAdmin.CreateAsync(adminUser, "!Scam2014");
                if (result.Succeeded)
                    await userManagerAdmin.AddToRoleAsync(adminUser, "Admin");
            }

        #endregion
        
        #region Create Manager Account

            var userManagerManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var managerUser = new AppUser { FirstName = "System", LastName = "Manager", UserName = "manager@domain.com", Email = "manager@domain.com" };

            var managerExists = await userManagerManager.Users.AnyAsync(u => u.Email == managerUser.Email);
            if (!managerExists)
            {
                var result = await userManagerManager.CreateAsync(managerUser, "!Scam2014");
                if (result.Succeeded)
                    await userManagerManager.AddToRoleAsync(managerUser, "Manager");
            }

        #endregion
        
        #region Create User Account

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = new AppUser { FirstName = "Test", LastName = "User", UserName = "user@domain.com", Email = "user@domain.com" };

            var userExists = await userManager.Users.AnyAsync(u => u.Email == user.Email);
            if (!userExists)
            {
                var result = await userManager.CreateAsync(user, "!Scam2014");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }

        #endregion
    }

#endregion

app.MapStaticAssets();

// app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/admin/overview"));
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();