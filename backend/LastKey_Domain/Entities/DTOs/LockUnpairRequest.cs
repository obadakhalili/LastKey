namespace LastKey_Domain.Entities.DTOs;

public class LockUnpairRequest
{
    public int LockId { get; set; }

    public int UserId { get; set; }
}