namespace PokemonRecommender.Models;

public class RateLimit
{
    public Guid UsuarioId { get; private set; }
    public int UsoAtual { get; private set; }
    public DateTime InicioJanela { get; private set; }
    public RateLimitPolicy Policy { get; private set; } = default!;

    private RateLimit() { }

    public RateLimit(RateLimitPolicy policy)
    {
        Policy = policy;
        UsoAtual = 0;
        InicioJanela = DateTime.UtcNow;
    }
}