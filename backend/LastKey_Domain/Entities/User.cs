namespace LastKey_Domain.Entities;

public record User 
{
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string UserImage { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public bool IsAdmin { get; set; }
}