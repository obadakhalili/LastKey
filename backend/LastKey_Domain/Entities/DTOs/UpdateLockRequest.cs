namespace LastKey_Domain.Entities.DTOs;

public class UpdateLockRequest
{
    public int UserId { get; set; }

    public int LockId { get; set; }
}