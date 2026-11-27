using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class AdminRepository(AppDbContext db) : IAdminRepository
{
    public Task<Admin?> BuscarPorIdAsync(Guid id, CancellationToken ct = default)
        => db.Usuarios.OfType<Admin>().FirstOrDefaultAsync(a => a.Id == id, ct);
}