using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiting.Playground.Infrastructure.RateLimiting;

namespace RateLimiting.Playground.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    /// <summary>
    /// Envio de notificaciones con una política de rate limiting razonable para mensajes. 100 requests por minuto. 
    /// </summary>
    /// <returns></returns>
    [HttpPost("send")]
    [EnableRateLimiting(RateLimitPolicies.Message)]
    public IActionResult SendNotification()
    {
        return Ok(new
        {
            message = "Notification queued",
            timestamp = DateTime.UtcNow
        });
    }
}
