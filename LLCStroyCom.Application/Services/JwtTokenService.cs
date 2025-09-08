using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LLCStroyCom.Domain.Configs;
using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Response;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LLCStroyCom.Application.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    
    public async Task<JwtTokenDto> CreateTokensAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        var accessToken = await CreateAccessTokenAsync(user);
        var refreshToken = await CreateRefreshTokenAsync();
        
        return await Task.FromResult(new JwtTokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
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

    private async Task<RefreshToken> CreateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(randomNumber);
        
        var token = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshToken()
        {
            TokenHash = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
        };
        
        return await Task.FromResult(refreshToken);
    }
}