using Pgvector;

namespace PokemonRecommender.Models;

public class Regra
{
    public Guid Id { get; private set; }
    public string Texto { get; private set; } = default!;
    public string Fonte { get; private set; } = default!;
    public Vector? Embedding { get; private set; }

    private Regra() { }

    public Regra(Guid id, string texto, string fonte, Vector? embedding = null)
    {
        Id = id;
        Texto = texto;
        Fonte = fonte;
        Embedding = embedding;
    }
}