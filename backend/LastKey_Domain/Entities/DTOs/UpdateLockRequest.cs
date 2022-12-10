namespace LastKey_Domain.Entities.DTOs;

public class UpdateLockRequest
{
    public int UserId { get; set; }

    public int LockId { get; set; }
    
    public LockProperties PropertyToUpdate { get; set; }

    public string? NewName { get; set; }

    public bool? IsLocked { get; set; }
}