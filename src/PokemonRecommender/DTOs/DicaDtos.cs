using System.ComponentModel.DataAnnotations;
using PokemonRecommender.Models;

namespace PokemonRecommender.DTOs;

public sealed record CartaDto
{
    public Guid Id { get; init; }

    [Required, MaxLength(100)]
    public string Nome { get; init; } = default!;

    public List<string> Tipos { get; init; } = [];
    public List<string> Movimentos { get; init; } = [];
}

public sealed record LadoBatalhaDto
{
    public List<CartaDto> Campo { get; init; } = [];
    public List<CartaDto> Mao { get; init; } = [];

    [Range(0, int.MaxValue)]
    public int Hp { get; init; }
}

public sealed record EstadoBatalhaRequest
{
    [Required]
    public LadoBatalhaDto Jogador { get; init; } = default!;

    [Required]
    public LadoBatalhaDto Oponente { get; init; } = default!;
}

public sealed record JogadaResponse(string Acao, float ScoreEsperado);

public sealed record DicaResponse(string Texto, DateTime GeradaEm, List<JogadaResponse> Jogadas);

public static class EstadoBatalhaMapper
{
    public static BattleState ParaModelo(this EstadoBatalhaRequest r)
        => new(ParaLado(r.Jogador), ParaLado(r.Oponente));

    private static LadoBatalha ParaLado(LadoBatalhaDto d)
        => new(d.Campo.Select(ParaCarta).ToList(), d.Mao.Select(ParaCarta).ToList(), d.Hp);

    // Os tipos são normalizados para minúsculo, igual aos nomes da PokeAPI (fire, water...).
    private static Carta ParaCarta(CartaDto c)
        => new(
            c.Id,
            c.Nome.Trim(),
            c.Tipos.Select(t => t.Trim().ToLowerInvariant()).ToList(),
            c.Movimentos);
}