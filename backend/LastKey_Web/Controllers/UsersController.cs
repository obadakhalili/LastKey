using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers;

[Controller]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        var createdUser = await _userService.CreateUserAsync(request);

        return Ok(createdUser);
    }
}