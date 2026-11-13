namespace PokemonRecommender.Models;

public sealed record RelacoesTipo(
    string Nome,
    IReadOnlyList<string> DobroDanoEm,
    IReadOnlyList<string> MetadeDanoEm,
    IReadOnlyList<string> SemDanoEm);