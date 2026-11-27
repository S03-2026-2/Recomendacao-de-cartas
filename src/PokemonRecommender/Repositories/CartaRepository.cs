using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class CartaRepository(AppDbContext db) : ICartaRepository
{
    public Task<Carta?> BuscarPorIdAsync(Guid id, CancellationToken ct = default)
        => db.Cartas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task SalvarAsync(Carta carta, CancellationToken ct = default)
    {
        if (db.Entry(carta).State == EntityState.Detached)
            db.Cartas.Add(carta);

        await db.SaveChangesAsync(ct);
    }
}