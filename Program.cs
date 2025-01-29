using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVCEcommerce.Data;
using MVCEcommerce.Services;

var builder = WebApplication.CreateBuilder(args);
//read EnvironmentVariables
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.


// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Para ver logs en la consola
builder.Logging.AddDebug();   // Para depuraci�n
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CloudConnection")));

// for posgresql   options.UseNpgsql(connectionString)
//for sqllite
//options.UseSqlite("Data Source=ecommercemvc.db")
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

try
{
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error cr�tico en la aplicaci�n");

    // Asegurar que la aplicaci�n no termine silenciosamente
    throw;
}
