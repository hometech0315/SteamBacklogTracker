using Microsoft.EntityFrameworkCore;
using SteamBacklogTracker.Application.Services;
using SteamBacklogTracker.Core.Interfaces;
using SteamBacklogTracker.Infrastructure.Data;
using SteamBacklogTracker.Infrastructure.Repositories;
using SteamBacklogTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Entity Framework
builder.Services.AddDbContext<SteamBacklogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();

// Add HTTP client for Steam API
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISteamApiService>(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var steamApiKey = configuration["SteamApi:ApiKey"] ?? throw new InvalidOperationException("Steam API Key is required");
    return new SteamApiService(httpClient, steamApiKey);
});

// Add Epic Games service
builder.Services.AddScoped<IEpicGamesService, EpicGamesService>();

// Add application services
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SteamBacklogContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
