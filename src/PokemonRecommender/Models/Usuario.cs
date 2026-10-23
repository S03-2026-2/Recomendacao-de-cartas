namespace PokemonRecommender.Models;

public abstract class Usuario
{
    public Guid Id { get; protected set; }
    public string Nome { get; protected set; } = default!;

    protected Usuario() { }

    protected Usuario(Guid id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}