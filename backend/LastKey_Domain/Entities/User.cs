using System.ComponentModel.DataAnnotations.Schema;

namespace LastKey_Domain.Entities;

public record User
{
    [Column("id")]
    public int UserId { get; set; }

    [Column("name")]
    public string FullName { get; set; }
    
    [Column("image")]
    public string UserImage { get; set; }

    [Column("username")]
    public string UserName { get; set; }

    [Column("password")]
    public string Password { get; set; }

    [Column("is_admin")]
    public bool IsAdmin { get; set; }

    public List<Lock> Locks { get; set; } = new();
    
    public int? AdminId { get; set; }
}