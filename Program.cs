using Microsoft.EntityFrameworkCore;
using Packer.Data;
using Packer.Models;
using Packer.Repositories;
using Packer.Services;

var migrate = args is ["--migrate"];
var builder = WebApplication.CreateBuilder(migrate ? [] : args);

builder.Services.AddDbContext<PackerDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Packer")
        ?? builder.Configuration["PACKER_POSTGRES_CONNECTION_STRING"];
    options.UseNpgsql(connectionString);
});

builder.Services.AddIdentityApiEndpoints<User>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<PackerDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddScoped<GearRepository>();
builder.Services.AddScoped<GearService>();
builder.Services.AddScoped<TripRepository>();
builder.Services.AddScoped<TripService>();
builder.Services.AddScoped<TripGearRepository>();
builder.Services.AddScoped<TripGearService>();
builder.Services.AddScoped<TripFoodRepository>();
builder.Services.AddScoped<TripFoodService>();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

if (migrate)
{
    await using var migrationApp = builder.Build();
    await using var scope = migrationApp.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<PackerDbContext>();
    await db.Database.MigrateAsync();
    return;
}

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("/api/auth").MapIdentityApi<User>();

app.Run();
