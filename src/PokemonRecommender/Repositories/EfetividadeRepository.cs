using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class EfetividadeRepository(AppDbContext db) : IEfetividadeRepository
{
    public Task<bool> ExisteAlgumaAsync(CancellationToken ct = default)
        => db.Efetividades.AnyAsync(ct);

    public async Task<IReadOnlyList<EfetividadeTipo>> ListarAsync(CancellationToken ct = default)
        => await db.Efetividades.AsNoTracking().ToListAsync(ct);

    public async Task AdicionarAsync(IEnumerable<EfetividadeTipo> itens, CancellationToken ct = default)
    {
        db.Efetividades.AddRange(itens);
        await db.SaveChangesAsync(ct);
    }
}