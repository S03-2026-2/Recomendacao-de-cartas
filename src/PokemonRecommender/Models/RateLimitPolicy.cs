namespace PokemonRecommender.Models;

public class RateLimitPolicy
{
    public int LimiteAtual { get; private set; }
    public TimeSpan JanelaTempo { get; private set; }

    private RateLimitPolicy() { }

    public RateLimitPolicy(int limiteAtual, TimeSpan janelaTempo)
    {
        LimiteAtual = limiteAtual;
        JanelaTempo = janelaTempo;
    }
}