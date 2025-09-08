using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Configs;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Key = "super-secret-key-with-min-32-characters-length-123",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 30,
            RefreshTokenExpirationDays = 7
        };

        var jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

        _jwtTokenService = new JwtTokenService(jwtSettingsMock.Object);
    }

    private ApplicationUser CreateTestUser(string email = "test@example.com", string role = "User")
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            Role = new ApplicationRole { Type = role }
        };
    }

    [Fact]
    public async Task CreateTokensAsync_ValidUser_ReturnsJwtTokenDtoWithBothTokens()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JwtTokenDto>(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.NotNull(result.RefreshToken.TokenHash);
    }

    [Fact]
    public async Task CreateTokensAsync_NullUser_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _jwtTokenService.CreateTokensAsync(null));
    }

    [Fact]
    public async Task CreateTokensAsync_AccessToken_ContainsUserEmailClaim()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com", "Admin");

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        // Assert
        var emailClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
    }

    [Fact]
    public async Task CreateTokensAsync_AccessToken_ContainsUserRoleClaim()
    {
        // Arrange
        var user = CreateTestUser("admin@example.com", "Administrator");

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        // Assert
        var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal(user.Role.Type, roleClaim.Value);
    }

    [Fact]
    public async Task CreateTokensAsync_AccessToken_HasCorrectIssuerAndAudience()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        // Assert
        Assert.Equal(_jwtSettings.Issuer, token.Issuer);
        Assert.Equal(_jwtSettings.Audience, token.Audiences.First());
    }

    [Fact]
    public async Task CreateTokensAsync_AccessToken_HasCorrectExpiration()
    {
        // Arrange
        var user = CreateTestUser();
        var expectedExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        // Assert
        Assert.True(token.ValidTo <= expectedExpiration.AddMinutes(1));
        Assert.True(token.ValidTo >= expectedExpiration.AddMinutes(-1));
    }

    [Fact]
    public async Task CreateTokensAsync_RefreshToken_HasCorrectExpiration()
    {
        // Arrange
        var user = CreateTestUser();
        var expectedExpiration = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);

        // Assert
        Assert.True(result.RefreshToken.ExpiresAt <= expectedExpiration.AddMinutes(1));
        Assert.True(result.RefreshToken.ExpiresAt >= expectedExpiration.AddMinutes(-1));
    }

    [Fact]
    public async Task CreateTokensAsync_RefreshToken_IsNotEmpty()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);

        // Assert
        Assert.NotNull(result.RefreshToken.TokenHash);
        Assert.NotEmpty(result.RefreshToken.TokenHash);
        Assert.True(result.RefreshToken.TokenHash.Length >= 32); // Base64 of 32 bytes
    }

    [Fact]
    public async Task CreateTokensAsync_DifferentUsers_ReturnDifferentAccessTokens()
    {
        // Arrange
        var user1 = CreateTestUser("user1@example.com", "User");
        var user2 = CreateTestUser("user2@example.com", "Admin");

        // Act
        var result1 = await _jwtTokenService.CreateTokensAsync(user1);
        var result2 = await _jwtTokenService.CreateTokensAsync(user2);

        // Assert
        Assert.NotEqual(result1.AccessToken, result2.AccessToken);
    }

    [Fact]
    public async Task CreateTokensAsync_RefreshTokens_AreUnique()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var result1 = await _jwtTokenService.CreateTokensAsync(user);
        var result2 = await _jwtTokenService.CreateTokensAsync(user);

        // Assert - Refresh tokens should be different each time
        Assert.NotEqual(result1.RefreshToken.TokenHash, result2.RefreshToken.TokenHash);
    }

    [Fact]
    public async Task CreateTokensAsync_AccessToken_CanBeValidatedWithCorrectKey()
    {
        // Arrange
        var user = CreateTestUser();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true
        };

        // Act
        var result = await _jwtTokenService.CreateTokensAsync(user);
        var handler = new JwtSecurityTokenHandler();

        // Assert - Should not throw exception
        var principal = handler.ValidateToken(result.AccessToken, validationParameters, out _);
        Assert.NotNull(principal);
    }

    [Fact]
    public async Task CreateTokensAsync_WithDifferentUserRoles_IncludeCorrectRoleClaims()
    {
        // Arrange
        var adminUser = CreateTestUser("admin@test.com", "Administrator");
        var moderatorUser = CreateTestUser("moderator@test.com", "Moderator");
        var regularUser = CreateTestUser("user@test.com", "User");

        // Act
        var adminResult = await _jwtTokenService.CreateTokensAsync(adminUser);
        var moderatorResult = await _jwtTokenService.CreateTokensAsync(moderatorUser);
        var userResult = await _jwtTokenService.CreateTokensAsync(regularUser);

        // Assert
        var adminToken = new JwtSecurityTokenHandler().ReadJwtToken(adminResult.AccessToken);
        var moderatorToken = new JwtSecurityTokenHandler().ReadJwtToken(moderatorResult.AccessToken);
        var userToken = new JwtSecurityTokenHandler().ReadJwtToken(userResult.AccessToken);

        Assert.Equal("Administrator", adminToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal("Moderator", moderatorToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal("User", userToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
    }
}