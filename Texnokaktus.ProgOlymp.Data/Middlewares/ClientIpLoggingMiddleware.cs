using Texnokaktus.ProgOlymp.Data.Extensions;

namespace Texnokaktus.ProgOlymp.Data.Middlewares;

internal class ClientIpLoggingMiddleware(ILogger<ClientIpLoggingMiddleware> logger, RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Handling request from {ClientIp}", context.GetClientRealIpAddress());

        return next.Invoke(context);
    }
}
