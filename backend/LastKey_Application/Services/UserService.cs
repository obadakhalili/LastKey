using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using LastKey_Application.Helpers;
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

        var b64UserImage = await request.UserImage.ToBase64ImageAsync();

        userModel = userModel with
        {
            IsAdmin = true,
            UserImage = b64UserImage,
            Password = GenerateEncryptedPassword(userModel.Password)
        };

        var createdUser = _mapper.Map<User>(await _userRepository.CreateUserAsync(userModel));

        return createdUser;
    }

    private string GenerateEncryptedPassword(string password)
    {
        var sha256 = SHA256.Create();

        var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));

        var encryptedPassword = Convert.ToBase64String(hash);

        return encryptedPassword;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userRepository.UsernameExistsAsync(username);
    }

    public async Task<AuthenticationResponse?> AuthenticateUserAsync(LoginUserRequest request)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);

        if (user == default)
            return null;
        
        var passwordMatched = GenerateEncryptedPassword(request.Password) == user.Password;

        if (!passwordMatched)
            return null;

        var jwtToken = GenerateUserToken(user);

        return new AuthenticationResponse
        {
            User = _mapper.Map<User>(user),
            JwtHeader = jwtToken.Item1,
            JwtPayload = jwtToken.Item3,
            JwtSignature = jwtToken.Item2
        };
    }

    private (string, string, string) GenerateUserToken(LastKey_Domain.Entities.User user)
    {
        var claims = new List<Claim>
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.IsAdmin ? nameof(Roles.Admin) : nameof(Roles.User))
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            Expires = DateTime.UtcNow.AddYears(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwtToken = tokenHandler.WriteToken(token).Split(".");

        var jwtTokenHeader = jwtToken[0];

        var jwtTokenSignature = jwtToken[2];

        var jwtTokenPayload = jwtToken[1];

        return (jwtTokenHeader, jwtTokenSignature, jwtTokenPayload);
    }

    public async Task<User?> RetrieveUserInfoByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserInfoByIdAsync(userId);

        return _mapper.Map<User>(user);
    }

    public void ClearCookies()
    {
        var cookies = _httpContext.HttpContext!.Request.Cookies;
        var cookiesToClear = _httpContext.HttpContext!.Response.Cookies;

        if (!cookies.ContainsKey("jwtHeader")) return;
        
        cookiesToClear.Delete("jwtHeader");
        cookiesToClear.Delete("jwtSignature");
        cookiesToClear.Delete("jwtPayload");
    }

    public async Task<User> AddMemberToUserAsync(int userId, CreateUserRequest request)
    {
        var userModel = _mapper.Map<LastKey_Domain.Entities.User>(request);

        var b64UserImage = await request.UserImage.ToBase64ImageAsync();

        userModel = userModel with
        {
            IsAdmin = false,
            UserImage = b64UserImage,
            Password = GenerateEncryptedPassword(userModel.Password),
            AdminId = userId
        };

        var createdUser = _mapper.Map<User>(await _userRepository.CreateUserAsync(userModel));

        return createdUser;
    }
}