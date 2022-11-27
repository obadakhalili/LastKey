namespace LastKey_Domain.Entities.DTOs;

public class User
{
    public int UserId { get; set; }
    
    public string FullName { get; set; }

    public string Username { get; set; }

    public bool IsAdmin { get; set; }
}