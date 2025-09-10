using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Services;

public interface IRefreshTokenService
{
    Task<JwtTokenDto> RefreshAsync(string plainRefreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(RefreshToken token, CancellationToken cancellationToken = default);
}