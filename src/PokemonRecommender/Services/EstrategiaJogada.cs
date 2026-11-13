using PokemonRecommender.Models;

namespace PokemonRecommender.Services;

public interface EstrategiaJogada
{
    Task<Jogada> CalcularAsync(BattleState estado, IReadOnlyList<Regra> regras, CancellationToken ct = default);
}