using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface ISolicitacaoRateLimitService
{
    Task<Resultado<SolicitacaoRateLimit>> CriarAsync(Guid jogadorId, int limiteSolicitado, CancellationToken ct = default);
    Task<Resultado<SolicitacaoRateLimit>> AprovarAsync(Guid id, CancellationToken ct = default);
    Task<Resultado<SolicitacaoRateLimit>> NegarAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SolicitacaoRateLimit>> ListarAsync(StatusSolicitacao? status, CancellationToken ct = default);
}

public class SolicitacaoRateLimitService(
    ISolicitacaoRepository solicitacoes,
    IPlayerRepository jogadores,
    IRateLimitRepository rateLimits) : ISolicitacaoRateLimitService
{
    public async Task<Resultado<SolicitacaoRateLimit>> CriarAsync(
        Guid jogadorId, int limiteSolicitado, CancellationToken ct = default)
    {
        if (limiteSolicitado <= 0)
            return Falha(StatusOperacao.Invalido, "O limite solicitado deve ser maior que zero.");

        if (await jogadores.BuscarPorIdAsync(jogadorId, ct) is null)
            return Falha(StatusOperacao.NaoEncontrado, "Jogador não encontrado.");

        if (await solicitacoes.ExistePendenteDoJogadorAsync(jogadorId, ct))
            return Falha(StatusOperacao.Conflito, "O jogador já tem uma solicitação pendente.");

        var solicitacao = new SolicitacaoRateLimit(Guid.NewGuid(), jogadorId, limiteSolicitado);
        await solicitacoes.SalvarAsync(solicitacao, ct);

        return Resultado<SolicitacaoRateLimit>.Novo(solicitacao);
    }

    public async Task<Resultado<SolicitacaoRateLimit>> AprovarAsync(Guid id, CancellationToken ct = default)
    {
        var solicitacao = await solicitacoes.BuscarPorIdAsync(id, ct);

        if (solicitacao is null)
            return Falha(StatusOperacao.NaoEncontrado, "Solicitação não encontrada.");

        if (solicitacao.Status != StatusSolicitacao.Pendente)
            return Falha(StatusOperacao.Conflito, "A solicitação já foi analisada.");

        var rateLimit = await rateLimits.BuscarAsync(solicitacao.JogadorId, ct);
        if (rateLimit is null)
            return Falha(StatusOperacao.NaoEncontrado, "Rate limit do jogador não encontrado.");

        rateLimit.Policy.DefinirLimite(solicitacao.LimiteSolicitado);
        solicitacao.DefinirStatus(StatusSolicitacao.Aprovada);

        // As duas entidades são rastreadas pelo mesmo DbContext, então esta gravação
        // persiste o novo limite e o novo status juntos, na mesma transação.
        await solicitacoes.SalvarAsync(solicitacao, ct);

        return Resultado<SolicitacaoRateLimit>.Sucesso(solicitacao);
    }

    public async Task<Resultado<SolicitacaoRateLimit>> NegarAsync(Guid id, CancellationToken ct = default)
    {
        var solicitacao = await solicitacoes.BuscarPorIdAsync(id, ct);

        if (solicitacao is null)
            return Falha(StatusOperacao.NaoEncontrado, "Solicitação não encontrada.");

        if (solicitacao.Status != StatusSolicitacao.Pendente)
            return Falha(StatusOperacao.Conflito, "A solicitação já foi analisada.");

        solicitacao.DefinirStatus(StatusSolicitacao.Negada);
        await solicitacoes.SalvarAsync(solicitacao, ct);

        return Resultado<SolicitacaoRateLimit>.Sucesso(solicitacao);
    }

    public Task<IReadOnlyList<SolicitacaoRateLimit>> ListarAsync(
        StatusSolicitacao? status, CancellationToken ct = default)
        => solicitacoes.ListarAsync(status, ct);

    private static Resultado<SolicitacaoRateLimit> Falha(StatusOperacao status, string mensagem)
        => Resultado<SolicitacaoRateLimit>.Falha(status, mensagem);
}