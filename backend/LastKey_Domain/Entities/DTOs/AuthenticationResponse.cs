namespace LastKey_Domain.Entities.DTOs;

public class AuthenticationResponse
{
    public User? User { get; set; }

    public string? JwtHeader { get; set; }

    public string? JwtPayload { get; set; }

    public string? JwtSignature { get; set; }
}