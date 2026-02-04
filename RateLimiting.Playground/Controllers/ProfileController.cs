using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiting.Playground.Infrastructure.RateLimiting;

namespace RateLimiting.Playground.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    /// <summary>
    /// Endpoint para usuarios ANÓNIMOS.
    /// Comparte la misma policy que el endpoint autenticado, pero la configuración del rate limiter aplica un límite menor.
    /// 60 req/min
    /// </summary>
    [HttpGet("public")]
    [EnableRateLimiting(RateLimitPolicies.Normal)]
    public IActionResult GetPublicProfile()
    {
        return Ok(new
        {
            authenticated = false,
            user = "anonymous",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint para usuarios AUTENTICADOS.
    /// La policy es la misma (Normal), pero el rate limiter detecta el usuario autenticado y permite más requests.
    /// 150 req/min
    /// </summary>
    [HttpGet("me")]
    [Authorize] // necesario para que Identity.IsAuthenticated sea true
    [EnableRateLimiting(RateLimitPolicies.Normal)]
    public IActionResult GetMyProfile()
    {
        return Ok(new
        {
            authenticated = true,
            user = User.Identity?.Name ?? "unknown",
            timestamp = DateTime.UtcNow
        });
    }
}
