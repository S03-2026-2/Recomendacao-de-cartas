using PokemonRecommender.Integrations;
using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

public interface IImportacaoPokeApiService
{
    Task ImportarTiposAsync(CancellationToken ct = default);
}

public class ImportacaoPokeApiService(
    IPokeApiClient pokeApi,
    IEfetividadeRepository efetividades,
    IRegraRepository regras,
    ILogger<ImportacaoPokeApiService> logger) : IImportacaoPokeApiService
{
    private const string PrefixoFonte = "pokeapi/type";

    public async Task ImportarTiposAsync(CancellationToken ct = default)
    {
        var precisaEfetividades = !await efetividades.ExisteAlgumaAsync(ct);
        var precisaRegras = !await regras.ExistemDaFonteAsync(PrefixoFonte, ct);

        if (!precisaEfetividades && !precisaRegras)
        {
            logger.LogInformation("Tipos da PokeAPI já importados, nada a fazer.");
            return;
        }

        logger.LogInformation("Importando tipos da PokeAPI...");
        var tipos = await BuscarTiposComRelacoesAsync(ct);

        if (precisaEfetividades)
            await efetividades.AdicionarAsync(GerarEfetividades(tipos), ct);

        if (precisaRegras)
            await regras.AdicionarAsync(GerarRegras(tipos), ct);

        logger.LogInformation("Importação concluída: {Quantidade} tipos.", tipos.Count);
    }

    private async Task<List<RelacoesTipo>> BuscarTiposComRelacoesAsync(CancellationToken ct)
    {
        var resultado = new List<RelacoesTipo>();

        foreach (var nome in await pokeApi.ListarTiposAsync(ct))
        {
            var tipo = await pokeApi.BuscarTipoAsync(nome, ct);
            var semRelacoes = tipo.DobroDanoEm.Count + tipo.MetadeDanoEm.Count + tipo.SemDanoEm.Count == 0;

            // Tipos como "unknown" e "shadow" vêm sem relações e não servem pra batalha.
            if (!semRelacoes)
                resultado.Add(tipo);
        }

        return resultado;
    }

    private static List<EfetividadeTipo> GerarEfetividades(IEnumerable<RelacoesTipo> tipos)
    {
        var lista = new List<EfetividadeTipo>();

        foreach (var t in tipos)
        {
            lista.AddRange(t.DobroDanoEm.Select(d => new EfetividadeTipo(t.Nome, d, 2.0)));
            lista.AddRange(t.MetadeDanoEm.Select(d => new EfetividadeTipo(t.Nome, d, 0.5)));
            lista.AddRange(t.SemDanoEm.Select(d => new EfetividadeTipo(t.Nome, d, 0.0)));
        }

        return lista;
    }

    private static List<Regra> GerarRegras(IEnumerable<RelacoesTipo> tipos)
    {
        var lista = new List<Regra>();

        foreach (var t in tipos)
        {
            var fonte = $"{PrefixoFonte}/{t.Nome}";
            AdicionarRegra(lista, fonte, t.Nome, "causa dano dobrado (2x) em", t.DobroDanoEm);
            AdicionarRegra(lista, fonte, t.Nome, "causa metade do dano (0.5x) em", t.MetadeDanoEm);
            AdicionarRegra(lista, fonte, t.Nome, "não causa dano (0x) em", t.SemDanoEm);
        }

        return lista;
    }

    private static void AdicionarRegra(
        List<Regra> lista, string fonte, string atacante, string efeito, IReadOnlyList<string> alvos)
    {
        if (alvos.Count == 0)
            return;

        var texto = $"Tipo {atacante} {efeito}: {string.Join(", ", alvos)}.";
        lista.Add(new Regra(Guid.NewGuid(), texto, fonte));
    }
}