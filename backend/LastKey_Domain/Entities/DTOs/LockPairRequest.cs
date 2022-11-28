namespace LastKey_Domain.Entities.DTOs;

public class LockPairRequest
{
    public int AdminId { get; set; }

    public string LockMacAddress { get; set; }

    public string LockName { get; set; }
}