using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface IRegraService
{
    Task<IReadOnlyList<Regra>> BuscarRelevantesAsync(BattleState estado, CancellationToken ct = default);
}

// Por enquanto as regras relevantes são as dos tipos presentes na partida.
// Quando houver embedding, a busca por similaridade (pgvector) entra aqui.
public class RegraService(IRegraRepository repository) : IRegraService
{
    // Mesmo prefixo usado pelo ImportacaoPokeApiService ao gravar as regras.
    private const string PrefixoFonte = "pokeapi/type";

    public async Task<IReadOnlyList<Regra>> BuscarRelevantesAsync(
        BattleState estado, CancellationToken ct = default)
    {
        var fontes = TodasAsCartas(estado)
            .SelectMany(c => c.Tipos)
            .Distinct()
            .Select(tipo => $"{PrefixoFonte}/{tipo}")
            .ToList();

        if (fontes.Count == 0)
            return [];

        return await repository.BuscarPorFontesAsync(fontes, ct);
    }

    private static IEnumerable<Carta> TodasAsCartas(BattleState e)
        => e.Jogador.Campo.Concat(e.Jogador.Mao).Concat(e.Oponente.Campo).Concat(e.Oponente.Mao);
}