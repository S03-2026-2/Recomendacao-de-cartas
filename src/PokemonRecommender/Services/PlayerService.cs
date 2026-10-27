using Microsoft.Extensions.Options;
using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public class PlayerService(
    IPlayerRepository repository,
    IOptions<RateLimitOptions> options) : IPlayerService
{
    public async Task<ResultadoCadastroJogador> SalvarNovoJogadorAsync(
        Guid id, string nome, string nickname, CancellationToken ct = default)
    {
        if (await repository.BuscarPorIdAsync(id, ct) is not null)
            return ResultadoCadastroJogador.JaExistia;

        var nicknameLimpo = nickname.Trim();
        if (await repository.NicknameEmUsoAsync(nicknameLimpo, ct))
            return ResultadoCadastroJogador.NicknameEmUso;

        var policy = new RateLimitPolicy(options.Value.LimiteInicial, options.Value.Janela);
        var jogador = new Jogador(id, nome.Trim(), nicknameLimpo, policy);

        await repository.SalvarAsync(jogador, ct);
        return ResultadoCadastroJogador.Criado;
    }
}