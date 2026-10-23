namespace PokemonRecommender.Models;

public class Carta
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = default!;
    public List<string> Tipos { get; private set; } = [];
    public List<string> Movimentos { get; private set; } = [];

    private Carta() { }

    public Carta(Guid id, string nome, List<string> tipos, List<string> movimentos)
    {
        Id = id;
        Nome = nome;
        Tipos = tipos;
        Movimentos = movimentos;
    }
}