using AutoMapper;
using LastKey_Application.DTOs.User;
using LastKey_Application.Services.User;
using LastKey_Domain.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers.Users;

[Controller]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserForCreationDto userToCreate)
    {
        var userModel = _mapper.Map<User>(userToCreate);
        
        var createdUser = await _userService.CreateUserAsync(userModel);

        return Ok(_mapper.Map<UserDto>(createdUser));
    }
}