using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Response;

namespace LLCStroyCom.Domain.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(string email, string password, string roleName, CancellationToken cancellationToken = default);
    Task<Result<JwtTokenDto>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<JwtTokenDto>> RefreshTokenAsync(JwtTokenDto tokens);
}