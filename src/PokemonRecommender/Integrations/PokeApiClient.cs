using System.Net.Http.Json;
using System.Text.Json;
using PokemonRecommender.Models;

namespace PokemonRecommender.Integrations;

public class PokeApiClient(HttpClient http) : IPokeApiClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<IReadOnlyList<string>> ListarTiposAsync(CancellationToken ct = default)
    {
        var resposta = await http.GetFromJsonAsync<ListaApi>("type?limit=100", Json, ct)
            ?? throw new InvalidOperationException("Resposta vazia da PokeAPI ao listar os tipos.");

        return resposta.Results.Select(r => r.Name).ToList();
    }

    public async Task<RelacoesTipo> BuscarTipoAsync(string nome, CancellationToken ct = default)
    {
        var tipo = await http.GetFromJsonAsync<TipoApi>($"type/{nome}", Json, ct)
            ?? throw new InvalidOperationException($"Resposta vazia da PokeAPI para o tipo '{nome}'.");

        var r = tipo.DamageRelations;
        return new RelacoesTipo(
            tipo.Name,
            Nomes(r.DoubleDamageTo),
            Nomes(r.HalfDamageTo),
            Nomes(r.NoDamageTo));
    }

    private static List<string> Nomes(List<ReferenciaApi>? itens)
        => itens?.Select(i => i.Name).ToList() ?? [];

    private sealed record ListaApi(List<ReferenciaApi> Results);

    private sealed record ReferenciaApi(string Name);

    private sealed record TipoApi(string Name, RelacoesApi DamageRelations);

    private sealed record RelacoesApi(
        List<ReferenciaApi>? DoubleDamageTo,
        List<ReferenciaApi>? HalfDamageTo,
        List<ReferenciaApi>? NoDamageTo);
}