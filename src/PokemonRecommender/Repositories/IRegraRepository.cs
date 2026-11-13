using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface IRegraRepository
{
    Task<bool> ExistemDaFonteAsync(string prefixoFonte, CancellationToken ct = default);
    Task AdicionarAsync(IEnumerable<Regra> regras, CancellationToken ct = default);
}