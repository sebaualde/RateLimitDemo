using Microsoft.AspNetCore.HttpOverrides;

namespace RateLimiting.Playground.Infrastructure.Networking;

public static class ForwardedHeadersExtensions
{
    /// <summary>
    /// Configura el soporte para headers reenviados por proxies (Cloudflare, Nginx, etc.)
    /// Permite obtener la IP real del cliente en lugar de la IP del proxy.
    /// </summary>
    public static IServiceCollection AddCloudflareForwardedHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            // Cloudflare actúa como reverse proxy.
            // Se limpian las redes conocidas para aceptar los headers reenviados.
            // IMPORTANTE: esto asume que la app SOLO es accesible a través del proxy.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }

    /// <summary>
    /// Habilita el middleware de forwarded headers.
    /// Debe ejecutarse ANTES de cualquier middleware que dependa de la IP del cliente.
    /// </summary>
    public static IApplicationBuilder UseCloudflareForwardedHeaders(this IApplicationBuilder app)
    {      
        return app.UseForwardedHeaders();
    }

}
