using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using User = LastKey_Domain.Entities.DTOs.User;

namespace LastKey_Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContext;

    public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContext)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
        _httpContext = httpContext;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var userModel = _mapper.Map<LastKey_Domain.Entities.User>(request);
        
        userModel = userModel with
        {
            IsAdmin = true
        };
        

        return _mapper.Map<User>(await _userRepository.CreateUserAsync(userModel));
    }

    public async Task<User?> AuthenticateUserAsync(LoginUserRequest request)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.username);

        if (user == default)
            return null;
        
        var passwordMatched = request.password == user.Password;

        if (!passwordMatched)
            return null;

        var jwtToken = GenerateUserToken(user);
        
        _httpContext.HttpContext!.Response.Cookies.Append("jwtToken", jwtToken, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1),
            Secure = true,
            HttpOnly = true
        });
        
        return _mapper.Map<User>(user);
    }

    private string GenerateUserToken(LastKey_Domain.Entities.User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Member")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
    
}