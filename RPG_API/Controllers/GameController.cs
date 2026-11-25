
using Microsoft.AspNetCore.Mvc;
using Game.Api.Services;

namespace Game.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    // GameService ontvangen via de constructor
    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    // Week 1: API-status controleren
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
        var spelInformatie = _gameService.GetInfo();

        return Ok(spelInformatie);
    }
}
