using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class PgVectorRegraRepository(AppDbContext db) : IRegraRepository
{
    public Task<bool> ExistemDaFonteAsync(string prefixoFonte, CancellationToken ct = default)
        => db.Regras.AnyAsync(r => r.Fonte.StartsWith(prefixoFonte), ct);

    public async Task<IReadOnlyList<Regra>> BuscarPorFontesAsync(
        IReadOnlyCollection<string> fontes, CancellationToken ct = default)
        => await db.Regras
            .AsNoTracking()
            .Where(r => fontes.Contains(r.Fonte))
            .ToListAsync(ct);

    public async Task AdicionarAsync(IEnumerable<Regra> regras, CancellationToken ct = default)
    {
        db.Regras.AddRange(regras);
        await db.SaveChangesAsync(ct);
    }
}