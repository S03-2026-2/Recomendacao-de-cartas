using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface IBloqueioDicaService
{
    Task<Resultado<Jogador>> BloquearAsync(Guid jogadorId, CancellationToken ct = default);
}

public class BloqueioDicaService(IPlayerRepository jogadores) : IBloqueioDicaService
{
    public async Task<Resultado<Jogador>> BloquearAsync(Guid jogadorId, CancellationToken ct = default)
    {
        var jogador = await jogadores.BuscarPorIdAsync(jogadorId, ct);

        if (jogador is null)
            return Resultado<Jogador>.Falha(StatusOperacao.NaoEncontrado, "Jogador não encontrado.");

        // Idempotente: bloquear quem já está bloqueado não gera erro.
        if (!jogador.DicaBloqueada)
        {
            jogador.BloquearDica();
            await jogadores.SalvarAsync(jogador, ct);
        }

        return Resultado<Jogador>.Sucesso(jogador);
    }
}