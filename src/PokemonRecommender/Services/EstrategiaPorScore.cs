using PokemonRecommender.Models;
using PokemonRecommender.Repositories;

namespace PokemonRecommender.Services;

// Escolhe a carta (em campo ou na mão) com maior vantagem de tipo contra a
// primeira carta do oponente em campo. O parâmetro "regras" não é usado por esta estratégia.
public class EstrategiaPorScore(IEfetividadeRepository efetividades) : EstrategiaJogada
{
    public async Task<Jogada> CalcularAsync(
        BattleState estado, IReadOnlyList<Regra> regras, CancellationToken ct = default)
    {
        var tabela = await CarregarTabelaAsync(ct);
        var alvo = estado.Oponente.Campo[0];

        var candidatas = estado.Jogador.Campo
            .Select(c => (Carta: c, Acao: $"Atacar com {c.Nome}"))
            .Concat(estado.Jogador.Mao.Select(c => (Carta: c, Acao: $"Jogar {c.Nome}")))
            .ToList();

        if (candidatas.Count == 0)
            throw new ArgumentException("O jogador não tem cartas disponíveis.");

        var melhor = candidatas
            .Select(x => (x.Carta, x.Acao, Score: Pontuar(x.Carta, alvo, tabela)))
            .MaxBy(x => x.Score);

        return new Jogada(melhor.Acao, (float)melhor.Score, melhor.Carta);
    }

    private async Task<Dictionary<(string Atacante, string Defensor), double>> CarregarTabelaAsync(
        CancellationToken ct)
    {
        var itens = await efetividades.ListarAsync(ct);

        if (itens.Count == 0)
            throw new DadosIndisponiveisException("Os tipos da PokeAPI ainda não foram importados.");

        return itens.ToDictionary(i => (i.Atacante, i.Defensor), i => i.Multiplicador);
    }

    // Maior multiplicador entre os tipos do atacante, multiplicando pelos tipos do alvo.
    private static double Pontuar(
        Carta atacante, Carta alvo, IReadOnlyDictionary<(string, string), double> tabela)
    {
        if (atacante.Tipos.Count == 0)
            return 1.0;

        return atacante.Tipos.Max(tipoAtacante =>
            alvo.Tipos.Aggregate(1.0, (acc, tipoAlvo) =>
                acc * Multiplicador(tabela, tipoAtacante, tipoAlvo)));
    }

    // Par que não está na tabela é dano normal (1x).
    private static double Multiplicador(
        IReadOnlyDictionary<(string, string), double> tabela, string atacante, string defensor)
        => tabela.TryGetValue((atacante, defensor), out var m) ? m : 1.0;
}