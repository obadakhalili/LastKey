using Microsoft.AspNetCore.Http;

namespace LastKey_Domain.Entities.DTOs;

public class CreateUserRequest
{
    public string FullName { get; set; }
    
    public IFormFile UserImage { get; set; }
    
    public string Username { get; set; }

    public string Password { get; set; }
}