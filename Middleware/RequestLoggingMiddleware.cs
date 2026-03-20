namespace NNPensionPlanner.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var startTime = DateTime.UtcNow;
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("[{CorrelationId}] {Method} {Path} - Started", correlationId, method, path);

        await _next(context);

        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("[{CorrelationId}] {Method} {Path} - {StatusCode} ({Duration:F1}ms)",
            correlationId, method, path, statusCode, duration);
    }
}
