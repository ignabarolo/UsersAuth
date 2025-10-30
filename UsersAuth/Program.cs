
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using UsersAuth.Common;
using UsersAuth.Identity;
using UsersAuth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppIdentityDBContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<User, Rol>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppIdentityDBContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireRole("Admin");
    });
});

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();
try
{
    await EnsureDatabaseIsSeeded(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while migrating or seeding the database.");
}

async Task EnsureDatabaseIsSeeded(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var services = scope.ServiceProvider;

        // 1. Migraci�n asegurada:
        var context = services.GetRequiredService<AppIdentityDBContext>();
        await context.Database.MigrateAsync(); // Usamos la versi�n AS�NCRONA

        // 2. Seeding (ahora s� se ejecuta despu�s de la migraci�n)
        var roleManager = services.GetRequiredService<RoleManager<Rol>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new Rol { Name = roleName });
            }
        }

        var adminEmail = app.Configuration.GetSection("AdminUser").GetSection("Email").Value;
        if (string.IsNullOrEmpty(adminEmail)) throw new Exception("Admin email is not configured.");

        var adminUserEmail = adminEmail;

        var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
        if (adminUser != null)
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}

app.MapRazorPages();
app.UseStaticFiles();

app.Run();
