using Microsoft.AspNetCore.Http;

namespace LastKey_Application.Helpers;

public static class UserHelper
{
    public static async Task<string> ToBase64ImageAsync(this IFormFile file)
    {
        var ms = new MemoryStream();

        await file.CopyToAsync(ms);

        var userImageFileBytes = ms.ToArray();

        return Convert.ToBase64String(userImageFileBytes);
    }
}