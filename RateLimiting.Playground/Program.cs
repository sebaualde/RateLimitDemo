using Microsoft.AspNetCore.Authentication;
using RateLimiting.Playground.Infrastructure.Auth;
using RateLimiting.Playground.Infrastructure.Networking;
using RateLimiting.Playground.Infrastructure.RateLimiting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// --------------------
// Service configuration
// --------------------

builder.Services
    .AddCloudflareForwardedHeaders()
    .AddOptimizedRateLimiter();

builder.Services
    .AddAuthentication(FakeAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(FakeAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddControllers();
builder.Services.AddOpenApi();


WebApplication app = builder.Build();

// --------------------
// Middleware configuration
// --------------------

// DEBE ir primero para obtener la IP real del cliente
app.UseCloudflareForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseAuthentication();

// Depende de la IP y del routing
app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
