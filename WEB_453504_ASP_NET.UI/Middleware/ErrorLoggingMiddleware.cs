using Serilog;
using ILogger = Serilog.ILogger;

namespace WEB_453504_ASP_NET.UI.Middleware;

/// <summary>
/// Middleware для логирования всех HTTP запросов с кодами состояния отличными от 2XX (OK)
/// </summary>
public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Передаём запрос следующему компоненту pipeline
            await _next(context);

            // Проверяем код состояния ответа
            // Если код не 2XX (успешный), логируем информацию
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
            {
                var url = context.Request.Path.Value;
                var statusCode = context.Response.StatusCode;
                
                _logger.Information("---> request {Url} returns {StatusCode}", url, statusCode);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}

/// <summary>
/// Extension методы для добавления middleware в pipeline
/// </summary>
public static class ErrorLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorLoggingMiddleware>();
    }
}
