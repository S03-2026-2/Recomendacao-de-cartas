using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class PlayerRepository(AppDbContext db) : IPlayerRepository
{
    public Task<Jogador?> BuscarPorIdAsync(Guid id, CancellationToken ct = default)
        => db.Usuarios
            .OfType<Jogador>()
            .Include(j => j.RateLimit)
            .FirstOrDefaultAsync(j => j.Id == id, ct);

    public Task<bool> NicknameEmUsoAsync(string nickname, CancellationToken ct = default)
        => db.Usuarios
            .OfType<Jogador>()
            .AnyAsync(j => j.Nickname == nickname, ct);

    public async Task SalvarAsync(Jogador jogador, CancellationToken ct = default)
    {
        db.Usuarios.Add(jogador);
        await db.SaveChangesAsync(ct);
    }
}