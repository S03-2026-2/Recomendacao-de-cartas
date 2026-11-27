using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface ICartaService
{
    Task<Resultado<Carta>> AdicionarAsync(
        Guid? id, string nome, IEnumerable<string> tipos, IEnumerable<string> movimentos,
        CancellationToken ct = default);

    Task<Carta?> BuscarPorIdAsync(Guid id, CancellationToken ct = default);
}

public class CartaService(ICartaRepository cartas) : ICartaService
{
    public async Task<Resultado<Carta>> AdicionarAsync(
        Guid? id, string nome, IEnumerable<string> tipos, IEnumerable<string> movimentos,
        CancellationToken ct = default)
    {
        var cartaId = id is { } informado && informado != Guid.Empty ? informado : Guid.NewGuid();

        if (await cartas.BuscarPorIdAsync(cartaId, ct) is not null)
            return Resultado<Carta>.Falha(StatusOperacao.Conflito, "Já existe uma carta com esse id.");

        // Tipos em minúsculo, como na PokeAPI.
        var tiposNormalizados = tipos
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => t.Length > 0)
            .Distinct()
            .ToList();

        if (tiposNormalizados.Count == 0)
            return Resultado<Carta>.Falha(StatusOperacao.Invalido, "A carta precisa ter pelo menos um tipo.");

        var movimentosLimpos = movimentos
            .Select(m => m.Trim())
            .Where(m => m.Length > 0)
            .ToList();

        var carta = new Carta(cartaId, nome.Trim(), tiposNormalizados, movimentosLimpos);
        await cartas.SalvarAsync(carta, ct);

        return Resultado<Carta>.Novo(carta);
    }

    public Task<Carta?> BuscarPorIdAsync(Guid id, CancellationToken ct = default)
        => cartas.BuscarPorIdAsync(id, ct);
}