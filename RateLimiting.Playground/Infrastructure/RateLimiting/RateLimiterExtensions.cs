using System.Threading.RateLimiting;

namespace RateLimiting.Playground.Infrastructure.RateLimiting;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddOptimizedRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // ? 1?? POLICY: GenerousLimit - Consultas públicas (productos, categorías, búsquedas)
            // Sin autenticación, alta velocidad - para navegación general
            options.AddPolicy(RateLimitPolicies.Generous, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 300,           // 300 peticiones por minuto
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10              // 10 en cola
                    }));

            // ? 2?? POLICY: NormalLimit - Usuarios registrados (carrito, órdenes, perfil)
            // Límites diferenciados por autenticación
            options.AddPolicy(RateLimitPolicies.Normal, context =>
            {
                // Obtener ID del usuario de múltiples fuentes
                string userId = context.User?.FindFirst("sub")?.Value ??                    // OpenID Connect
                               context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? // Claims
                               context.User?.Identity?.Name ??                             // Username
                               context.Connection.RemoteIpAddress?.ToString() ??
                               "anonymous";

                bool isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"user_{userId}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = isAuthenticated ? 150 : 60,  // 150 autenticados, 60 anónimos
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = isAuthenticated ? 5 : 2       // 5 cola autenticados, 2 anónimos
                    });
            });

            // ? 3?? POLICY: StrictLimit - Autenticación y operaciones críticas
            // MUY RESTRICTIVO - protección contra fuerza bruta y operaciones destructivas
            options.AddPolicy(RateLimitPolicies.Strict, context =>
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,                           // Solo 5 intentos por IP
                        Window = TimeSpan.FromMinutes(1),          // 1 minuto
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0                             // Sin cola - rechazar directamente
                    });
            });

            // ? 4?? POLICY: ModerateLimit - Operaciones sensibles pero frecuentes
            // Balance entre seguridad y usabilidad
            options.AddPolicy(RateLimitPolicies.Moderate, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,                          // 20 intentos por IP
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0                             // Sin cola
                    }));

            // ? 5?? POLICY: HighThroughputLimit - Webhooks (PayPal, MercadoPago)
            // ALTO VOLUMEN - Pueden venir en ráfagas
            options.AddPolicy(RateLimitPolicies.HighThroughput, context =>
            {
                // Usar IP del servidor de webhooks como partición
                string webhookSource = context.Request.Headers["X-Webhook-Source"].ToString() ??
                                      context.Request.Headers["X-Forwarded-For"].ToString() ??
                                      context.Connection.RemoteIpAddress?.ToString() ??
                                      "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"webhook_{webhookSource}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 500,                         // 500 webhooks por minuto
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 50                            // 50 en cola
                    });
            });

            // ? 6?? POLICY: MessageLimit - Notificaciones (emails, SMS, push notifications)
            // Sin restricción extrema pero controlado
            options.AddPolicy(RateLimitPolicies.Message, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,                         // 100 notificaciones por minuto
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    }));

            // ? Manejo de respuesta cuando se excede el límite
            options.OnRejected = async (context, cancellationToken) =>
            {
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status429TooManyRequests;
                response.ContentType = "application/json";

                // Obtener tiempo de reintento si está disponible
                int retryAfterSeconds = 60;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    retryAfterSeconds = (int)retryAfter.TotalSeconds;
                    response.Headers.RetryAfter = retryAfterSeconds.ToString();
                }

                var errorResponse = new
                {
                    statusCode = 429,
                    message = "Límite de peticiones excedido. Intente más tarde.",
                    retryAfter = retryAfterSeconds,
                    timestamp = DateTime.UtcNow,
                    ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                await response.WriteAsJsonAsync(errorResponse, cancellationToken);
            };
        });

        return services;
    }
}

