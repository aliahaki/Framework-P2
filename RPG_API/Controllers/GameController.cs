
using Microsoft.AspNetCore.Mvc;
using Game.Api.Services;

namespace Game.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    // GameService ontvangen via dependency injection
    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    // Controleren of de API werkt
    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            message = "API werkt"
        });
    }

    // Informatie over Quiz Challenge ophalen
    [HttpGet("info")]
    public IActionResult Info()
    {
        return Ok(_gameService.GetInfo());
    }

    // Alle quizvragen ophalen
    [HttpGet("vragen")]
    public IActionResult Vragen()
    {
        return Ok(_gameService.GetVragen());
    }
}
