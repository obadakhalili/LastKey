namespace LastKey_Domain.Entities.DTOs;

public class UpdateLockRequest
{
    public int AdminId { get; set; }

    public int LockId { get; set; }

    public string NewName { get; set; }
}