namespace RateLimiting.Playground.Infrastructure.RateLimiting;

/// <summary>
/// Define las claves de políticas de Rate Limiting basadas en el NIVEL DE RESTRICCIÓN.
/// Los nombres describen la restricción, no el caso de uso, para evitar confusiones.
/// 
/// Mapeo de restricciones:
/// - StrictLimit (5 req/min): Operaciones críticas de seguridad
/// - ModerateLimit (20 req/min): Operaciones sensibles pero frecuentes
/// - NormalLimit (150/60 req/min): Operaciones normales de usuarios
/// - GenerousLimit (300 req/min): Consultas públicas sin autenticación
/// - HighThroughputLimit (500 req/min): Webhooks y eventos externos
/// - MessageLimit (100 req/min): Notificaciones internas
/// </summary>
public static class RateLimitPolicies
{
    /// <summary>
    /// MUY RESTRICTIVO (5 req/min)
    /// Operaciones críticas de seguridad:
    /// - Login, Registro, Reset Password
    /// - 2FA, Cambio de contraseña
    /// - Moderar comentarios (admin)
    /// - Eliminar cuentas
    /// - Crear/Eliminar admins
    /// - Crear cupones
    /// </summary>
    public const string Strict = "strict";

    /// <summary>
    /// MODERADO (20 req/min)
    /// Operaciones sensibles pero frecuentes:
    /// - Marcar notificaciones como leídas
    /// - Crear/actualizar opiniones y reseñas
    /// - Cambiar direcciones en checkout
    /// - Cambios en carrito
    /// </summary>
    public const string Moderate = "moderate";

    /// <summary>
    /// NORMAL (150/60 req/min)
    /// Operaciones normales de usuarios:
    /// - Lectura de perfil, órdenes, direcciones
    /// - Consultas admin normales
    /// - Filtros y búsquedas
    /// - 150 req/min para autenticados
    /// - 60 req/min para anónimos
    /// </summary>
    public const string Normal = "normal";

    /// <summary>
    /// GENEROSO (300 req/min)
    /// Consultas públicas sin autenticación:
    /// - Productos, categorías, búsquedas
    /// - Catálogo público
    /// </summary>
    public const string Generous = "generous";

    /// <summary>
    /// ALTO VOLUMEN (500 req/min)
    /// Webhooks y eventos externos:
    /// - PayPal, MercadoPago webhooks
    /// - Pueden venir en ráfagas
    /// </summary>
    public const string HighThroughput = "high_throughput";

    /// <summary>
    /// MENSAJES (100 req/min)
    /// Notificaciones internas:
    /// - Emails, SMS, push notifications
    /// </summary>
    public const string Message = "message";
}
