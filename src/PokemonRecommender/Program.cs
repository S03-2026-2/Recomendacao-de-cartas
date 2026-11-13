using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Integrations;
using PokemonRecommender.Repositories;
using PokemonRecommender.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default"), n => n.UseVector()));

builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));

builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>(c =>
{
    c.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
    c.Timeout = TimeSpan.FromSeconds(15);
});

// Novo jogador
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Importação da PokeAPI
builder.Services.AddScoped<IEfetividadeRepository, EfetividadeRepository>();
builder.Services.AddScoped<IRegraRepository, PgVectorRegraRepository>();
builder.Services.AddScoped<IImportacaoPokeApiService, ImportacaoPokeApiService>();

// Pedir dica
builder.Services.AddScoped<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddScoped<IRateLimitService, RateLimitService>();
builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();
builder.Services.AddScoped<IRegraService, RegraService>();
builder.Services.AddScoped<EstrategiaJogada, EstrategiaPorScore>();
builder.Services.AddScoped<IDicaService, DicaService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

    try
    {
        await scope.ServiceProvider
            .GetRequiredService<IImportacaoPokeApiService>()
            .ImportarTiposAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex,
            "Não foi possível importar os tipos da PokeAPI. A API segue sem esses dados; reinicie para tentar de novo.");
    }
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();