namespace PokemonRecommender.Services;

public enum ResultadoCadastroJogador
{
    Criado,
    JaExistia,
    NicknameEmUso
}

public interface IPlayerService
{
    Task<ResultadoCadastroJogador> SalvarNovoJogadorAsync(
        Guid id, string nome, string nickname, CancellationToken ct = default);
}