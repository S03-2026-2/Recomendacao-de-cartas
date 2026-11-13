using PokemonRecommender.Models;

namespace PokemonRecommender.Integrations;

public interface IPokeApiClient
{
    Task<IReadOnlyList<string>> ListarTiposAsync(CancellationToken ct = default);
    Task<RelacoesTipo> BuscarTipoAsync(string nome, CancellationToken ct = default);
}