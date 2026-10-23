using Pgvector;

namespace PokemonRecommender.Models;

public class Regra
{
    public Guid Id { get; private set; }
    public string Texto { get; private set; } = default!;
    public Vector Embedding { get; private set; } = default!;
    public string Fonte { get; private set; } = default!;

    private Regra() { }

    public Regra(Guid id, string texto, Vector embedding, string fonte)
    {
        Id = id;
        Texto = texto;
        Embedding = embedding;
        Fonte = fonte;
    }
}