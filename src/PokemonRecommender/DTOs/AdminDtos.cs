using System.ComponentModel.DataAnnotations;
using PokemonRecommender.Models;

namespace PokemonRecommender.DTOs;

public sealed record CriarSolicitacaoRequest
{
    [Required]
    public Guid JogadorId { get; init; }

    [Range(1, 1000)]
    public int LimiteSolicitado { get; init; }
}

public sealed record SolicitacaoResponse(
    Guid Id, Guid JogadorId, int LimiteSolicitado, string Status, DateTime DataCriacao)
{
    public static SolicitacaoResponse De(SolicitacaoRateLimit s)
        => new(s.Id, s.JogadorId, s.LimiteSolicitado, s.Status.ToString(), s.DataCriacao);
}

public sealed record CriarCartaRequest
{
    public Guid? Id { get; init; }

    [Required, MaxLength(100)]
    public string Nome { get; init; } = default!;

    public List<string> Tipos { get; init; } = [];
    public List<string> Movimentos { get; init; } = [];
}

public sealed record CartaResponse(Guid Id, string Nome, List<string> Tipos, List<string> Movimentos)
{
    public static CartaResponse De(Carta c) => new(c.Id, c.Nome, c.Tipos, c.Movimentos);
}