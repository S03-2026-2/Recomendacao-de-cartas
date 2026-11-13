using System.Globalization;
using PokemonRecommender.Models;

namespace PokemonRecommender.Services;

public interface IDicaService
{
    Dica Montar(Jogada jogada, IReadOnlyList<Regra> regras);
    string Formatar(Dica dica);
}

public class DicaService : IDicaService
{
    public Dica Montar(Jogada jogada, IReadOnlyList<Regra> regras)
    {
        var tipos = jogada.Carta?.Tipos ?? new List<string>();

        // A explicação usa as regras dos tipos da carta recomendada.
        var explicacao = string.Join(" ", regras
            .Where(r => tipos.Any(t => r.Fonte.EndsWith($"/{t}")))
            .Select(r => r.Texto));

        return new Dica(jogada.Acao, explicacao, DateTime.UtcNow, [jogada]);
    }

    public string Formatar(Dica dica)
    {
        var score = dica.Jogadas[0].ScoreEsperado.ToString("0.##", CultureInfo.InvariantCulture);
        var texto = $"{dica.Texto} (efetividade estimada: {score}x)";

        return string.IsNullOrWhiteSpace(dica.Explicacao)
            ? texto
            : $"{texto}. Por quê: {dica.Explicacao}";
    }
}