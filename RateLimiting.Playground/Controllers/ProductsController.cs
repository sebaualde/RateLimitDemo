using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiting.Playground.Infrastructure.RateLimiting;

namespace RateLimiting.Playground.Controllers;

/**
 * Controlador para operaciones relacionadas con productos.
 * Aplica una política de rate limiting generosa para permitir un alto volumen 300 requests por minuto
 */

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting(RateLimitPolicies.Generous)]
    public IActionResult GetProducts()
    {
        return Ok(new
        {
            message = "Product list",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("{id}")]
    [EnableRateLimiting(RateLimitPolicies.Generous)]
    public IActionResult GetProductById(int id)
    {
        return Ok(new
        {
            productId = id,
            timestamp = DateTime.UtcNow
        });
    }
}
