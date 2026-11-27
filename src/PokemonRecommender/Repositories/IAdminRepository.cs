using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface IAdminRepository
{
    Task<Admin?> BuscarPorIdAsync(Guid id, CancellationToken ct = default);
}