namespace PokemonRecommender.Models;

public class Jogador : Usuario
{
    public string Nickname { get; private set; } = default!;
    public bool DicaBloqueada { get; private set; }
    public RateLimit RateLimit { get; private set; } = default!;

    private Jogador() { }

    public Jogador(Guid id, string nome, string nickname, RateLimitPolicy policyInicial)
        : base(id, nome)
    {
        Nickname = nickname;
        RateLimit = new RateLimit(policyInicial);
    }

    public void BloquearDica() => DicaBloqueada = true;
}