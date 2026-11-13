namespace PokemonRecommender.Models;

public record BattleState(LadoBatalha Jogador, LadoBatalha Oponente);

public record LadoBatalha(IReadOnlyList<Carta> Campo, IReadOnlyList<Carta> Mao, int Hp);