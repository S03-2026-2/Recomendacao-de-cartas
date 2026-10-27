using System.ComponentModel.DataAnnotations;

namespace PokemonRecommender.DTOs;

public sealed record NovoJogadorEvento
{
    [Required]
    public Guid PlayerId { get; init; }

    [Required, MaxLength(100)]
    public string Nome { get; init; } = default!;

    [Required, MaxLength(50)]
    public string Nickname { get; init; } = default!;
}