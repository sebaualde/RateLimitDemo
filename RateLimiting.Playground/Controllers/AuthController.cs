using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiting.Playground.Infrastructure.RateLimiting;

namespace RateLimiting.Playground.Controllers;

/**
 * Controlador para operaciones de autenticación.
 * Aplica una política de rate limiting estricta para proteger contra ataques de fuerza bruta.
 * más de 5 requests por minuto desde la misma IP resultado esperado: 429 Too Many Requests
 */
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.Strict)]
    public IActionResult Login()
    {
        return Ok(new
        {
            message = "Login attempt accepted",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitPolicies.Strict)]
    public IActionResult ResetPassword()
    {
        return Ok(new
        {
            message = "Reset password attempt accepted",
            timestamp = DateTime.UtcNow
        });
    }
}
