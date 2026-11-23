using Microsoft.EntityFrameworkCore;
using Packer.Data;
using Packer.Models;
using Packer.Repositories;
using Packer.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("/api/auth").MapIdentityApi<User>();

app.Run();
