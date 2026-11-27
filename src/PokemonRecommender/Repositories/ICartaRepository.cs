using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface ICartaRepository
{
    Task<Carta?> BuscarPorIdAsync(Guid id, CancellationToken ct = default);
    Task SalvarAsync(Carta carta, CancellationToken ct = default);
}