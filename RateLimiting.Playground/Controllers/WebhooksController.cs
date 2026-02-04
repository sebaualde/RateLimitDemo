using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiting.Playground.Infrastructure.RateLimiting;

namespace RateLimiting.Playground.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WebhooksController : ControllerBase
{
    /// <summary>
    /// Maneja los webhooks de pagos entrantes con una alta tasa de solicitudes permitidas. 500 requests por minuto. 
    /// </summary>
    /// <returns></returns>
    [HttpPost("payment")]
    [EnableRateLimiting(RateLimitPolicies.HighThroughput)]
    public IActionResult PaymentWebhook()
    {
        return Ok(new
        {
            message = "Webhook received",
            timestamp = DateTime.UtcNow
        });
    }
}
