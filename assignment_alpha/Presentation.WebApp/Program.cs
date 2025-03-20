// using Data.Contexts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.WebApp.Data;
using Presentation.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

#region Added to get the Service and Context working with Identity functionalities (SignUp / SignIn)

    builder.Services.AddDbContext<DataContext>(x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<Member, IdentityRole>(x =>
        {
            x.Password.RequiredLength = 8;
            x.User.RequireUniqueEmail = true;
            x.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<DataContext>()
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