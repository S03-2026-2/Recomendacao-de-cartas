namespace PokemonRecommender.Services;

public class RateLimitOptions
{
    public int LimiteInicial { get; set; } = 5;
    public TimeSpan Janela { get; set; } = TimeSpan.FromDays(1);
}