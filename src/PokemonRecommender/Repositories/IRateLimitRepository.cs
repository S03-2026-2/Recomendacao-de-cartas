using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface IRateLimitRepository
{
    Task<RateLimit?> BuscarAsync(Guid usuarioId, CancellationToken ct = default);
    Task SalvarAsync(RateLimit rateLimit, CancellationToken ct = default);
}