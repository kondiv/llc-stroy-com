using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Dto;

public class JwtTokenDto
{
    public string AccessToken { get; set; } = null!;
    public RefreshToken RefreshToken { get; set; } = null!;
}