using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface IRateLimitService
{
    Task<bool> VerificarAsync(Usuario usuario, CancellationToken ct = default);
    Task RegistrarUsoAsync(Usuario usuario, CancellationToken ct = default);
}

public class RateLimitService(IRateLimitRepository repository) : IRateLimitService
{
    public async Task<bool> VerificarAsync(Usuario usuario, CancellationToken ct = default)
    {
        var rateLimit = await repository.BuscarAsync(usuario.Id, ct);
        if (rateLimit is null)
            return false;

        return JanelaExpirada(rateLimit, DateTime.UtcNow)
            || rateLimit.UsoAtual < rateLimit.Policy.LimiteAtual;
    }

    public async Task RegistrarUsoAsync(Usuario usuario, CancellationToken ct = default)
    {
        var rateLimit = await repository.BuscarAsync(usuario.Id, ct)
            ?? throw new InvalidOperationException("Rate limit não encontrado para o usuário.");

        var agora = DateTime.UtcNow;
        if (JanelaExpirada(rateLimit, agora))
            rateLimit.ReiniciarJanela(agora);

        rateLimit.IncrementarUso();
        await repository.SalvarAsync(rateLimit, ct);
    }

    private static bool JanelaExpirada(RateLimit rateLimit, DateTime agora)
        => agora - rateLimit.InicioJanela >= rateLimit.Policy.JanelaTempo;
}