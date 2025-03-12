using Data.Contexts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// builder.Services.AddDbContext<DataContext>();

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