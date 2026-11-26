
using Microsoft.AspNetCore.Mvc;
using Game.Api.Services;
using Game.Api.Models;

namespace Game.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    // Service ontvangen via dependency injection
    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    // Week 1: status controleren
    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            message = "API werkt"
        });
    }

    // Week 2: spelinformatie ophalen
    [HttpGet("info")]
    public IActionResult Info()
    {
        return Ok(_gameService.GetInfo());
    }

    // Quizvragen ophalen
    [HttpGet("vragen")]
    public IActionResult Vragen()
    {
        return Ok(_gameService.GetVragen());
    }

    // Week 3: een nieuw spel starten
    [HttpPost]
    public async Task<ActionResult<GameResponse>> Start(
        StartGameRequest request)
    {
        try
        {
            // De spelregels staan in GameService
            GameResponse spel = await _gameService.StartAsync(request);

            return Ok(spel);
        }
        catch (ArgumentException uitzondering)
        {
            // Ongeldige spelernaam geeft HTTP 400
            return BadRequest(new
            {
                message = uitzondering.Message
            });
        }
    }
}
