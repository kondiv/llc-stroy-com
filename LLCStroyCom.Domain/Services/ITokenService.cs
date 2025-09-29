using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Services;

public interface ITokenService
{
    Task<JwtTokenDto> CreateTokensAsync(ApplicationUser user);
}