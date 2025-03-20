using Data.Contexts;
using Data.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.WebApp.Data;
using Presentation.WebApp.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

#region Added to get the Service and Context working with Identity functionalities (SignUp / SignIn)

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    // var dataConnection = builder.Configuration.GetConnectionString("DataConnection") ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");

    // builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // builder.Services.ConfigureApplicationCookie(x =>
    // {
    //     x.LoginPath = "/auth/signin";
    //     x.LogoutPath = "/auth/signout";
    //     x.AccessDeniedPath = "/auth/denied";
    //     x.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //     x.SlidingExpiration = true;
    // });

    // builder.Services.AddScoped<UserService>();

#endregion

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();