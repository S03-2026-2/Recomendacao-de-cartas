using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface IEfetividadeRepository
{
    Task<bool> ExisteAlgumaAsync(CancellationToken ct = default);
    Task<IReadOnlyList<EfetividadeTipo>> ListarAsync(CancellationToken ct = default);
    Task AdicionarAsync(IEnumerable<EfetividadeTipo> itens, CancellationToken ct = default);
}