namespace LastKey_Domain.Entities.DTOs;

public class Lock
{
    public int LockId { get; set; }

    public string LockName { get; set; }

    public string MacAddress { get; set; }

    public string Message { get; set; } = string.Empty;
}