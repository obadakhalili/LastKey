using LastKey_Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers;

[Authorize]
[Controller]
[Route("api/network")]
public class NetworkController : ControllerBase
{
    private readonly INetworkService _networkService;

    public NetworkController(INetworkService networkService)
    {
        _networkService = networkService;
    }

    [HttpGet("locks")]
    public async Task<ActionResult<List<string>>> RetrieveNetworkLocks()
    {
        var unregisteredLocks = await _networkService.RetrieveUnregisteredLocksAsync();

        if (unregisteredLocks == null)
            return Ok();

        return Ok(unregisteredLocks);
    }
}