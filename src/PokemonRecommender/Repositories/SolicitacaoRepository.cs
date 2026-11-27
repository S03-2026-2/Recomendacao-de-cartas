using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Data;
using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public class SolicitacaoRepository(AppDbContext db) : ISolicitacaoRepository
{
    public Task<SolicitacaoRateLimit?> BuscarPorIdAsync(Guid id, CancellationToken ct = default)
        => db.Solicitacoes.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<SolicitacaoRateLimit>> ListarAsync(
        StatusSolicitacao? status, CancellationToken ct = default)
    {
        var consulta = db.Solicitacoes.AsNoTracking();

        if (status is not null)
            consulta = consulta.Where(s => s.Status == status.Value);

        return await consulta.OrderBy(s => s.DataCriacao).ToListAsync(ct);
    }

    public Task<bool> ExistePendenteDoJogadorAsync(Guid jogadorId, CancellationToken ct = default)
        => db.Solicitacoes.AnyAsync(
            s => s.JogadorId == jogadorId && s.Status == StatusSolicitacao.Pendente, ct);

    public async Task SalvarAsync(SolicitacaoRateLimit solicitacao, CancellationToken ct = default)
    {
        if (db.Entry(solicitacao).State == EntityState.Detached)
            db.Solicitacoes.Add(solicitacao);

        await db.SaveChangesAsync(ct);
    }
}