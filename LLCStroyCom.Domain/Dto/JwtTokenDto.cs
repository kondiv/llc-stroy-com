using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Dto;

public class JwtTokenDto
{
    public string AccessToken { get; set; } = null!;
    public RefreshTokenDto RefreshTokenDto { get; set; } = null!;
}