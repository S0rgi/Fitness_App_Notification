using Microsoft.AspNetCore.Mvc;
namespace Fitness_App_Notification.Controllers;
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}