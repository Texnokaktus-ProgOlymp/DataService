using System.Net;

namespace Texnokaktus.ProgOlymp.Data.Extensions;

public static class HttpContextExtensions
{
    public static IPAddress? GetClientRealIpAddress(this HttpContext context) =>
        context.Request.GetRealIpAddress()?? context.Connection.RemoteIpAddress;
    
    private static IPAddress? GetRealIpAddress(this HttpRequest request) =>
        IPAddress.TryParse(request.Headers["X-Real-Ip"].FirstOrDefault(), out var ipAddress)
            ? ipAddress
            : null;
}
