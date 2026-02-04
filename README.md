# RateLimitDemo
Implementación de Rate Limiting en ASP.NET Core (.NET 8) detrás de Cloudflare

Proyecto de ejemplo que muestra una implementación **realista y ordenada de Rate Limiting en ASP.NET Core (.NET 8 LTS)**, incluyendo soporte para **Cloudflare / proxies mediante Forwarded Headers**.

La idea de este repo es servir como **documentación práctica**, evitando los típicos ejemplos simplificados que funcionan en un tutorial pero fallan cuando se llevan a un proyecto real.

---

## 🎯 Objetivo

Demostrar:

- Cómo configurar **Rate Limiting nativo** en ASP.NET Core
- Cómo definir **políticas reutilizables** basadas en niveles de restricción
- Cómo evitar *magic strings* usando constantes
- Cómo manejar correctamente **IP real detrás de Cloudflare**
- Por qué el **orden del middleware importa**
- Cómo probar distintas políticas con endpoints reales

---

## 🧱 Stack

- **.NET 8 (LTS)**
- **ASP.NET Core**
- Rate Limiting nativo (`Microsoft.AspNetCore.RateLimiting`)
- Cloudflare / Reverse Proxy support (`ForwardedHeaders`)

---

## 🧠 Enfoque

En lugar de definir políticas por caso de uso (`LoginPolicy`, `SearchPolicy`, etc.), este proyecto usa **políticas basadas en nivel de restricción**.

Esto permite:
- Reutilizar políticas
- Cambiar límites sin romper contratos
- Mantener coherencia en toda la API

---

## 🔑 RateLimitPolicies

Las políticas se definen como constantes para evitar errores de tipeo y facilitar cambios futuros:

```csharp
public static class RateLimitPolicies
{
    public const string Strict = "strict_limit";
    public const string Moderate = "moderate_limit";
    public const string Normal = "normal_limit";
    public const string Generous = "generous_limit";
    public const string HighThroughput = "high_throughput_limit";
    public const string Message = "message_limit";
}
```

Cada nombre describe **el nivel de restricción**, no el endpoint.

---

## ⚙️ Configuración de Rate Limiting

Toda la configuración se encapsula en una extensión para mantener `Program.cs` limpio: 
```csharp
builder.AddOptimizedRateLimiter();
```
La clase `RateLimiterConfig` define:

| Policy | Límite | Uso típico |
|------|------|----------|
| Strict | 5 req/min | Login, reset password, operaciones críticas |
| Moderate | 20 req/min | Cambios sensibles pero frecuentes |
| Normal | 150 / 60 req/min | Usuarios autenticados / anónimos |
| Generous | 300 req/min | Catálogo público |
| HighThroughput | 500 req/min | Webhooks |
| Message | 100 req/min | Emails / notificaciones |

Incluye además manejo personalizado del **HTTP 429** con `Retry-After`.

---

## ☁️ Cloudflare y Forwarded Headers

Cuando se usa Cloudflare (o cualquier proxy), la IP real del cliente **no llega directamente a la API**.

Este proyecto incluye una extensión dedicada:

```csharp
builder.ConfigureForwardedHeaders();
```
Esto asegura que:
- `HttpContext.Connection.RemoteIpAddress`
- Rate Limiting por IP
- Logs y auditoría

funcionen correctamente en producción.

---

## 🧩 Orden del Middleware

El orden es **crítico**. En este proyecto se usa:

```csharp
app.UseForwardedHeaders();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
```
Cambiar este orden puede provocar:
- IPs incorrectas
- Rate limiting inconsistente
- Errores de autenticación

---

## 🔌 Endpoints de prueba

Se incluyen endpoints simples para validar el comportamiento:

- Endpoint **anónimo** → `RateLimitPolicies.Generous`
- Endpoint **autenticado** → `RateLimitPolicies.Normal`

Esto permite probar rápidamente los límites usando herramientas como:
- curl
- Postman
- navegador

---

## ▶️ Cómo ejecutar

```console
dotnet run
```

El proyecto está pensado como **demo**, no como template productivo, pero aplica prácticas reales y probadas.

---

## 📝 Nota final

Este repo nace de la necesidad de tener un ejemplo **real**, con decisiones que normalmente no aparecen en tutoriales.

Si te resulta útil, genial.  
Si ves algo para mejorar, toda sugerencia es bienvenida.


