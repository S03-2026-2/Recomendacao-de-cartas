using Microsoft.AspNetCore.Mvc;
using PokemonRecommender.DTOs;
using PokemonRecommender.Services;

namespace PokemonRecommender.Controllers;

[ApiController]
[Route("api/dica")]
public class DicaController(IRecomendacaoService recomendacao) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PedirDica(EstadoBatalhaRequest request, CancellationToken ct)
    {
        var token = ExtrairToken();
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        try
        {
            var resultado = await recomendacao.GerarDicaAsync(token, request.ParaModelo(), ct);

            return resultado.Status switch
            {
                StatusRecomendacao.Ok => Ok(MontarResposta(resultado)),
                StatusRecomendacao.NaoAutenticado => Unauthorized(),
                StatusRecomendacao.DicaBloqueada =>
                    StatusCode(StatusCodes.Status403Forbidden, "Dicas bloqueadas para este jogador."),
                StatusRecomendacao.LimiteExcedido =>
                    StatusCode(StatusCodes.Status429TooManyRequests, "Limite de dicas atingido."),
                StatusRecomendacao.EstadoInvalido => UnprocessableEntity(resultado.Texto),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch (DadosIndisponiveisException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
    }

    private static DicaResponse MontarResposta(ResultadoRecomendacao resultado)
    {
        var dica = resultado.DicaGerada!;
        var jogadas = dica.Jogadas
            .Select(j => new JogadaResponse(j.Acao, j.ScoreEsperado))
            .ToList();

        return new DicaResponse(resultado.Texto!, dica.GeradaEm, jogadas);
    }

    private string ExtrairToken()
    {
        const string prefixo = "Bearer ";
        var header = Request.Headers.Authorization.ToString();

        return header.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase)
            ? header[prefixo.Length..].Trim()
            : header.Trim();
    }
}