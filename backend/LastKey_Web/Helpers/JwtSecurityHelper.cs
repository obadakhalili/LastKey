using System.IdentityModel.Tokens.Jwt;

namespace LastKey_Web.Helpers;

public static class JwtSecurityHelper
{
    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
    
    public static int GetUserIdFromToken(HttpRequest request)
    {
        var token = GetToken(request);
        var jsonToken = JwtSecurityTokenHandler.ReadJwtToken(token);
        var userId = jsonToken.Claims.First(claim => claim.Type == "userId")
            .Value;

        return int.Parse(userId);
    }
    
    private static string GetToken(HttpRequest request)
    {
        request.Cookies.TryGetValue("jwtHeader", out var jwtHeader);
        request.Cookies.TryGetValue("jwtPayload", out var jwtPayload);
        request.Cookies.TryGetValue("jwtSignature", out var jwtSignature);

        return $"{jwtHeader}.{jwtPayload}.{jwtSignature}";
    }
}