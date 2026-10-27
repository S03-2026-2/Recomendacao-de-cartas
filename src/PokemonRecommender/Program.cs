using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Repositories;
using PokemonRecommender.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default"), n => n.UseVector()));

builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();