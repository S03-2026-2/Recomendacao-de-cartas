using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface ISolicitacaoRepository
{
    Task<SolicitacaoRateLimit?> BuscarPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SolicitacaoRateLimit>> ListarAsync(StatusSolicitacao? status, CancellationToken ct = default);
    Task<bool> ExistePendenteDoJogadorAsync(Guid jogadorId, CancellationToken ct = default);
    Task SalvarAsync(SolicitacaoRateLimit solicitacao, CancellationToken ct = default);
}