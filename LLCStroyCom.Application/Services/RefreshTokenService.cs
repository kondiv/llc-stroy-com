using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;

namespace LLCStroyCom.Application.Services;

// TODO Write tests
public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<RefreshTokenService> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<JwtTokenDto> 
        RefreshAsync(string plainRefreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plainRefreshToken);
        
        var refreshToken = await _refreshTokenRepository.GetAsync(plainRefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            _logger.LogWarning("Refresh token is missing or revoked");
            throw new UnauthorizedException();
        }
        _logger.LogInformation("Refresh token is valid");
        
        var user = await _userRepository.GetAsync(refreshToken.UserId, cancellationToken);

        var tokens = await _tokenService.CreateTokensAsync(user);

        await _userRepository.AssignRefreshTokenAsync(user.Id, tokens.RefreshTokenDto.RefreshTokenEntity,
            cancellationToken);

        await RevokeAsync(refreshToken, cancellationToken);
        _logger.LogInformation($"Refresh token {refreshToken.Id} is revoked. New tokens are generated");

        return tokens;
    }

    public async Task RevokeAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        
        token.RevokedAt = DateTimeOffset.UtcNow;

        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
    }
}