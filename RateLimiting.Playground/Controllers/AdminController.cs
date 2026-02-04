using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiting.Playground.Infrastructure.RateLimiting;

namespace RateLimiting.Playground.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    /// <summary>
    /// Endpoint para acciones administrativas moderadamente limitadas de 20 solicitudes por minuto. 
    /// </summary>
    [HttpPost("moderate")]
    [EnableRateLimiting(RateLimitPolicies.Moderate)]
    public IActionResult ModerateContent()
    {
        return Ok(new
        {
            message = "Moderation action accepted",
            timestamp = DateTime.UtcNow
        });
    }
}
