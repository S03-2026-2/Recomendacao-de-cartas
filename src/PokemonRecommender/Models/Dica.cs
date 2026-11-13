namespace PokemonRecommender.Models;

public record Jogada(string Acao, float ScoreEsperado, Carta? Carta = null);

public record Dica(string Texto, string Explicacao, DateTime GeradaEm, IReadOnlyList<Jogada> Jogadas);