using PokemonRecommender.Models;

namespace PokemonRecommender.Repositories;

public interface IPlayerRepository
{
    Task<Jogador?> BuscarPorIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> NicknameEmUsoAsync(string nickname, CancellationToken ct = default);
    Task SalvarAsync(Jogador jogador, CancellationToken ct = default);
}