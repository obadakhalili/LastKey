using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using LastKey_Domain.Interfaces;

namespace LastKey_Application.Services;

public class NetworkService : INetworkService
{
    private readonly ILockRepository _lockRepository;

    public NetworkService(ILockRepository lockRepository)
    {
        _lockRepository = lockRepository;
    }
    
    public async Task<List<string>> RetrieveUnregisteredLocksAsync()
    {
        var registeredLocks = (await _lockRepository.RetrieveLocksAsync()).Select(l => l.MacAddress)
            .ToList();

        var unregisteredLocks = GetNetworkLocks().Where(l => !registeredLocks.Contains(l)).ToList();
        
        for(var i =0;i < unregisteredLocks.Count;i++)
        {
            unregisteredLocks[i] = Regex.Replace(unregisteredLocks[i], ".{2}(?!$)", "$0-");
        }

        return unregisteredLocks;
    }

    private IEnumerable<string> GetNetworkLocks()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up 
                          && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback 
                          && !nic.GetPhysicalAddress().Equals(PhysicalAddress.None))
            .Select(nic => nic.GetPhysicalAddress().ToString());
    }
}