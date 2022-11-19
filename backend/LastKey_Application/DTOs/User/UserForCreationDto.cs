using System.ComponentModel.DataAnnotations.Schema;

namespace LastKey_Application.DTOs.User;

public class UserForCreationDto
{
    public string FullName { get; set; }
    
    public string UserImage { get; set; }
    
    public string UserName { get; set; }

    public string Password { get; set; }
}