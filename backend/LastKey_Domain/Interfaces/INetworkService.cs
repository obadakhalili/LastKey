namespace LastKey_Domain.Interfaces;

public interface INetworkService
{
    Task<List<string>> RetrieveUnregisteredLocksAsync();
}