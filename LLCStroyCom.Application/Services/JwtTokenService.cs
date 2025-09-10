using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LLCStroyCom.Domain.Configs;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LLCStroyCom.Application.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ITokenHasher _tokenHasher;
    
    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        ITokenHasher tokenHasher)
    {
        _jwtSettings = jwtSettings.Value;
        _tokenHasher = tokenHasher;
    }

    public async Task<JwtTokenDto> CreateTokensAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var accessToken = await CreateAccessTokenAsync(user);
        var refreshToken = await CreateRefreshTokenAsync();

        return await Task.FromResult(new JwtTokenDto()
        {
            AccessToken = accessToken,
            RefreshTokenDto = refreshToken,
        });
    }

    private async Task<string> CreateAccessTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.Type)
        };
        
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: signingCredentials);
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return await Task.FromResult(tokenString);
    }

    private async Task<RefreshTokenDto> CreateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(randomNumber);
        
        var token = Convert.ToBase64String(randomNumber);
        var tokenHash = _tokenHasher.HashToken(token);
        
        var refreshToken = new RefreshToken()
        {
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
        };
        
        return await Task.FromResult(new RefreshTokenDto(refreshToken, token));
    }
}