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
    
    public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
    {
        if (await _userService.UsernameExistsAsync(request.UserName))
        {
            return BadRequest("Username Already Exists!");
        }
        
        var createdUser = await _userService.CreateUserAsync(request);

        return Ok(createdUser);
    }
}