namespace LastKey_Web.Helpers;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Cookies.ContainsKey("jwtHeader"))
        {
            var header = httpContext.Request.Cookies["jwtHeader"];
            var payload = httpContext.Request.Cookies["jwtPayload"];
            var signature = httpContext.Request.Cookies["jwtSignature"];
            var token = $"{header}.{payload}.{signature}";
            
            httpContext.Request.Headers.Append("Authorization", $"Bearer {token}");
        }

        await _next.Invoke(httpContext);
    }
}