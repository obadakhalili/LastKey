using LastKey_Domain.Entities;

namespace LastKey_Domain.Interfaces;

public interface ILockRepository
{
    Task<List<Lock>> RetrieveLocksAsync();
}