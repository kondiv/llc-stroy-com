using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class RefreshTokenServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenServiceTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        var loggerMock = new Mock<ILogger<RefreshTokenService>>();
        _refreshTokenService = new RefreshTokenService(
            _refreshTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            loggerMock.Object);
    }
    [Fact]
    public async Task RefreshAsync_WhenRefreshTokenIsNull_ShouldReturnValidTokens()
    {
        // Arrange
        string? plainRefreshToken = null;
        
        // Act
        var act = () => _refreshTokenService.RefreshAsync(plainRefreshToken);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenPlainRefreshTokenIsEmptyOrWhitespace_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var plainRefreshToken = string.Empty;

        // Act
        var act = () => _refreshTokenService.RefreshAsync(plainRefreshToken);
        
        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenRefreshTokenIsRevoked_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var plainRefreshToken = "plainRefreshToken";
        var refreshToken = new RefreshToken()
        {
            RevokedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
        };

        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(It.Is<string>(s => s == plainRefreshToken), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        
        // Act
        var act = () => _refreshTokenService.RefreshAsync(plainRefreshToken);
        
        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(act);
        _refreshTokenRepositoryMock
            .Verify(r =>
                    r.GetAsync(It.Is<string>(s => s == plainRefreshToken),
                        It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_WhenRefreshTokenIsNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var plainRefreshToken = "plainRefreshToken";

        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(It.Is<string>(s => s == plainRefreshToken), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken)null);
        
        // Act
        var act = () => _refreshTokenService.RefreshAsync(plainRefreshToken);
        
        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenUserWithIdStoredInRefreshTokenNotFound_ShouldThrowUserCouldNotBeFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plainRefreshToken = "plainRefreshToken";
        var refreshToken = new RefreshToken()
        {
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
        };

        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken); 
        
        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(UserCouldNotBeFound.WithId(userId));
        
        // Act
        var act = () => _refreshTokenService.RefreshAsync(plainRefreshToken);
        
        // Assert
        await Assert.ThrowsAsync<UserCouldNotBeFound>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        var plainRefreshToken = "plainRefreshToken";

        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<string>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _refreshTokenService.RefreshAsync(plainRefreshToken, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenRefreshTokenIsValid_ShouldCreateNewTokenPairAndRevokeOldRefreshToken()
    {
        // Arrange
        var plainRefreshToken = "plainRefreshToken";
        var userId = Guid.NewGuid();
        var refreshToken = new RefreshToken()
        {
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
        };
        var user = new ApplicationUser()
        {
            Id = userId,
        };
        
        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(s => s.CreateTokensAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new JwtTokenDto()
            {
                AccessToken = "accessToken",
                RefreshTokenDto = new RefreshTokenDto(new RefreshToken(), "plainRefreshToken")
            });
        
        // Act
        var tokens = await _refreshTokenService.RefreshAsync(plainRefreshToken);
        
        // Assert
        Assert.NotNull(tokens);
        Assert.NotNull(refreshToken.RevokedAt);
    }

    [Fact]
    public async Task RevokeAsync_WhenRefreshTokenIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        RefreshToken? refreshToken = null;
        
        // Act
        var act = () => _refreshTokenService.RevokeAsync(refreshToken);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task RevokeAsync_WhenRefreshTokenIsNotRevoked_ShouldRevokeToken()
    {
        // Arrange
        var refreshToken = new RefreshToken()
        {
            RevokedAt = null,
        };
        
        // Act
        await _refreshTokenService.RevokeAsync(refreshToken);
        
        // Assert
        Assert.NotNull(refreshToken.RevokedAt);
    }
}