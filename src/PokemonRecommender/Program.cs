using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using PokemonRecommender.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default"), n => n.UseVector()));

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();