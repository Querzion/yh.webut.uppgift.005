using Data.Contexts;
using Data.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presentation.WebApp.Data;
using Presentation.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// var connectionString = DatabaseHelper.GetConnectionString();
var applicationConnection = builder.Configuration.GetConnectionString("ApplicationConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationConnection' not found.");
var dataConnection = builder.Configuration.GetConnectionString("DataConnection") ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(dataConnection));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(applicationConnection));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();