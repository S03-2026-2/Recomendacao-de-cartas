using Microsoft.AspNetCore.Mvc;
using PokemonRecommender.DTOs;
using PokemonRecommender.Services;

namespace PokemonRecommender.Controllers;

[ApiController]
[Route("api/eventos")]
public class EventoController(IPlayerService playerService) : ControllerBase
{
    [HttpPost("novo-jogador")]
    public async Task<IActionResult> ReceberNovoJogador(
        NovoJogadorEvento evento, CancellationToken ct)
    {
        if (evento.PlayerId == Guid.Empty)
            return BadRequest("playerId inválido.");

        var resultado = await playerService.SalvarNovoJogadorAsync(
            evento.PlayerId, evento.Nome, evento.Nickname, ct);

        return resultado switch
        {
            ResultadoCadastroJogador.Criado =>
                StatusCode(StatusCodes.Status201Created, new { playerId = evento.PlayerId }),
            ResultadoCadastroJogador.JaExistia => Ok(new { playerId = evento.PlayerId }),
            ResultadoCadastroJogador.NicknameEmUso => Conflict("Nickname já está em uso."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}