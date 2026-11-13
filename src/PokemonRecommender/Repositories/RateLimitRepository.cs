using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class RateLimitRepository(AppDbContext db) : IRateLimitRepository
{
    public Task<RateLimit?> BuscarAsync(Guid usuarioId, CancellationToken ct = default)
        => db.RateLimits.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId, ct);

    public async Task SalvarAsync(RateLimit rateLimit, CancellationToken ct = default)
    {
        if (db.Entry(rateLimit).State == EntityState.Detached)
            db.RateLimits.Update(rateLimit);

        await db.SaveChangesAsync(ct);
    }
}