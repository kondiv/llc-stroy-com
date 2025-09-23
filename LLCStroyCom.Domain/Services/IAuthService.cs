using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Response;

namespace LLCStroyCom.Domain.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(string name, string email, string password, string roleName, CancellationToken cancellationToken = default);
    Task<Result<PlainJwtTokensDto>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}