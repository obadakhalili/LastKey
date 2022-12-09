using System.ComponentModel.DataAnnotations.Schema;

namespace LastKey_Domain.Entities;

public record Lock
{
    [Column("id")]
    public int LockId { get; set; }

    [Column("name")]
    public string LockName { get; set; }

    [Column("mac_address")]
    public string MacAddress { get; set; }

    [Column("is_locked")]
    public bool IsLocked { get; set; }
    
    [Column("admin_id")] 
    public int UserId { get; set; }
    public User User { get; set; }
}