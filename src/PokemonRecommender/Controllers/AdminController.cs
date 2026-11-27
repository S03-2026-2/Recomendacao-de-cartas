using Microsoft.AspNetCore.Mvc;
using PokemonRecommender.DTOs;
using PokemonRecommender.Models;
using PokemonRecommender.Services;

namespace PokemonRecommender.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController(
    IAutenticacaoService autenticacao,
    ISolicitacaoRateLimitService solicitacoes,
    IBloqueioDicaService bloqueio,
    ICartaService cartas) : ControllerBase
{
    [HttpPost("solicitacoes")]
    public async Task<IActionResult> CriarSolicitacao(CriarSolicitacaoRequest request, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var resultado = await solicitacoes.CriarAsync(request.JogadorId, request.LimiteSolicitado, ct);
        return ParaResposta(resultado, s => SolicitacaoResponse.De(s));
    }

    [HttpGet("solicitacoes")]
    public async Task<IActionResult> ListarSolicitacoes([FromQuery] StatusSolicitacao? status, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var lista = await solicitacoes.ListarAsync(status, ct);
        return Ok(lista.Select(s => SolicitacaoResponse.De(s)));
    }

    [HttpPost("solicitacoes/{id:guid}/aprovar")]
    public async Task<IActionResult> AprovarSolicitacao(Guid id, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var resultado = await solicitacoes.AprovarAsync(id, ct);
        return ParaResposta(resultado, s => SolicitacaoResponse.De(s));
    }

    [HttpPost("solicitacoes/{id:guid}/negar")]
    public async Task<IActionResult> NegarSolicitacao(Guid id, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var resultado = await solicitacoes.NegarAsync(id, ct);
        return ParaResposta(resultado, s => SolicitacaoResponse.De(s));
    }

    [HttpPost("jogadores/{id:guid}/bloquear-dica")]
    public async Task<IActionResult> BloquearDica(Guid id, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var resultado = await bloqueio.BloquearAsync(id, ct);

        if (resultado.Status == StatusOperacao.Ok)
            return NoContent();

        return ParaResposta(resultado, j => j.Id);
    }

    [HttpPost("cartas")]
    public async Task<IActionResult> AdicionarCarta(CriarCartaRequest request, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var resultado = await cartas.AdicionarAsync(
            request.Id, request.Nome, request.Tipos, request.Movimentos, ct);

        return ParaResposta(resultado, c => CartaResponse.De(c));
    }

    [HttpGet("cartas/{id:guid}")]
    public async Task<IActionResult> BuscarCarta(Guid id, CancellationToken ct)
    {
        if (await AutenticarAdminAsync(ct) is null)
            return Unauthorized();

        var carta = await cartas.BuscarPorIdAsync(id, ct);
        return carta is null ? NotFound("Carta não encontrada.") : Ok(CartaResponse.De(carta));
    }

    private IActionResult ParaResposta<T, TResposta>(Resultado<T> resultado, Func<T, TResposta> mapa)
        => resultado.Status switch
        {
            StatusOperacao.Ok => Ok(mapa(resultado.Valor!)),
            StatusOperacao.Criado => StatusCode(StatusCodes.Status201Created, mapa(resultado.Valor!)),
            StatusOperacao.NaoEncontrado => NotFound(resultado.Mensagem),
            StatusOperacao.Conflito => Conflict(resultado.Mensagem),
            StatusOperacao.Invalido => UnprocessableEntity(resultado.Mensagem),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };

    private async Task<Admin?> AutenticarAdminAsync(CancellationToken ct)
    {
        var token = ExtrairToken();

        return string.IsNullOrWhiteSpace(token)
            ? null
            : await autenticacao.AutenticarAdminAsync(token, ct);
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