
using Microsoft.AspNetCore.Mvc;

namespace Game.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    // Week 1: controleren of de API werkt
    [HttpGet("status")]
    public IActionResult Status()
    {
        // Een bericht teruggeven aan de gebruiker
        return Ok(new
        {
            message = "API werkt"
        });
    }
}

        