namespace LastKey_Domain.Entities;

public class Lock
{
    public int LockId { get; set; }

    public string LockName { get; set; }

    public string MacAddress { get; set; }

    public int? AdminId { get; set; }
    public User User { get; set; }
}