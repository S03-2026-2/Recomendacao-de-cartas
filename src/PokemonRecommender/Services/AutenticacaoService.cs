using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface IAutenticacaoService
{
    Task<Usuario?> AutenticarAsync(string token, CancellationToken ct = default);
    Task<Admin?> AutenticarAdminAsync(string token, CancellationToken ct = default);
}

// PROVISÓRIO: enquanto o formato do token não for combinado com os outros grupos,
// o token é o próprio id (Guid) do jogador ou do admin.
public class AutenticacaoService(IPlayerRepository players, IAdminRepository admins) : IAutenticacaoService
{
    public async Task<Usuario?> AutenticarAsync(string token, CancellationToken ct = default)
    {
        if (!Guid.TryParse(token, out var id))
            return null;

        return await players.BuscarPorIdAsync(id, ct);
    }

    public async Task<Admin?> AutenticarAdminAsync(string token, CancellationToken ct = default)
    {
        if (!Guid.TryParse(token, out var id))
            return null;

        return await admins.BuscarPorIdAsync(id, ct);
    }
}