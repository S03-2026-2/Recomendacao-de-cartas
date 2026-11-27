namespace PokemonRecommender.Services;

public enum StatusOperacao
{
    Ok,
    Criado,
    NaoEncontrado,
    Conflito,
    Invalido
}

public sealed record Resultado<T>(StatusOperacao Status, T? Valor = default, string? Mensagem = null)
{
    public static Resultado<T> Sucesso(T valor) => new(StatusOperacao.Ok, valor);
    public static Resultado<T> Novo(T valor) => new(StatusOperacao.Criado, valor);
    public static Resultado<T> Falha(StatusOperacao status, string mensagem) => new(status, default, mensagem);
}