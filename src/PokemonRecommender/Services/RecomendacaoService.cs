using PokemonRecommender.Models;

namespace PokemonRecommender.Services;

public enum StatusRecomendacao
{
    Ok,
    NaoAutenticado,
    DicaBloqueada,
    LimiteExcedido,
    EstadoInvalido
}

// Texto: a dica formatada (Ok) ou a mensagem de erro (EstadoInvalido).
public record ResultadoRecomendacao(StatusRecomendacao Status, Dica? DicaGerada = null, string? Texto = null);

public interface IRecomendacaoService
{
    Task<ResultadoRecomendacao> GerarDicaAsync(string token, BattleState estado, CancellationToken ct = default);
}

public class RecomendacaoService(
    IAutenticacaoService autenticacao,
    IRateLimitService rateLimit,
    IRegraService regraService,
    EstrategiaJogada estrategia,
    IDicaService dicaService) : IRecomendacaoService
{
    public async Task<ResultadoRecomendacao> GerarDicaAsync(
        string token, BattleState estado, CancellationToken ct = default)
    {
        if (await autenticacao.AutenticarAsync(token, ct) is not Jogador jogador)
            return new(StatusRecomendacao.NaoAutenticado);

        if (jogador.DicaBloqueada)
            return new(StatusRecomendacao.DicaBloqueada);

        var erro = Validar(estado);
        if (erro is not null)
            return new(StatusRecomendacao.EstadoInvalido, Texto: erro);

        if (!await rateLimit.VerificarAsync(jogador, ct))
            return new(StatusRecomendacao.LimiteExcedido);

        var regras = await regraService.BuscarRelevantesAsync(estado, ct);
        var jogada = await estrategia.CalcularAsync(estado, regras, ct);
        var dica = dicaService.Montar(jogada, regras);

        // O uso só é registrado depois de a dica sair: pedido inválido não gasta a cota.
        await rateLimit.RegistrarUsoAsync(jogador, ct);

        return new(StatusRecomendacao.Ok, dica, dicaService.Formatar(dica));
    }

    private static string? Validar(BattleState estado)
    {
        if (estado.Oponente.Campo.Count == 0)
            return "O oponente precisa ter uma carta em campo.";

        if (estado.Jogador.Campo.Count + estado.Jogador.Mao.Count == 0)
            return "O jogador precisa ter pelo menos uma carta no campo ou na mão.";

        return null;
    }
}