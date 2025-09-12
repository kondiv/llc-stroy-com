using System.Security.Claims;
using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Response;

namespace LLCStroyCom.Domain.Services;

public interface ITokenService
{
    Task<JwtTokenDto> CreateTokensAsync(ApplicationUser user);
}