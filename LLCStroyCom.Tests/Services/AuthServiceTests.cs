using LLCStroyCom.Application.Services;
using LLCStroyCom.Application.Validators.Auth;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;

    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        var authenticationDataValidator = new AuthenticationDataValidator();
        var loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _passwordHasherMock.Object,
            authenticationDataValidator,
            loggerMock.Object);
    }
    
    [Fact]
    public async Task RegisterAsync_WhenDataIsValid_ShouldReturnResultSuccess()
    {
        // Arrange
        var email = "email@email.com";
        var password = "Strong_Passw0rd";
        var roleName = "Engineer";
        var userId = Guid.NewGuid();

        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync(roleName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApplicationRole() { Type = roleName });

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        
        // Act
        var result = await _authService.RegisterAsync(email, password, roleName);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsInvalid_ShouldReturnResultFailure()
    {
        // Arrange
        var invalidEmail = "invalidEmail";
        var password = "Strong_Passw0rd";
        var roleName = "Engineer";
        var userId = Guid.NewGuid();
        
        // Act
        var result = await _authService.RegisterAsync(invalidEmail, password, roleName);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordIsInvalid_ShouldReturnResultFailure()
    {
        // Arrange
        var email = "email@email.ru";
        var invalidPassword = "invalidPassword";
        var roleName = "Engineer";
        var userId = Guid.NewGuid();
        
        // Act
        var result = await _authService.RegisterAsync(email, invalidPassword, roleName);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenRoleNameIsEmpty_ShouldReturnResultFailure()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Strong_Passw0rd";
        var roleName = string.Empty;

        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync(roleName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());
        
        // Act
        var result = await _authService.RegisterAsync(email, password, roleName);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenRoleNameIsNull_ShouldReturnResultFailure()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Strong_Passw0rd";
        string? roleName = null;
        
        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync(roleName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException());
        
        // Act
        var result = await _authService.RegisterAsync(email, password, roleName);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenRoleDoesNotExist_ShouldReturnResultFailure()
    {
        // Arrange
        var email = "email@email.com";
        var password = "Strong_Passw0rd";
        var roleName = "randomRole";
        
        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync(roleName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(RoleCouldNotBeFound.WithName(roleName));
        
        // Act
        var result = await _authService.RegisterAsync(email, password, roleName);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserIsNull_ShouldReturnResultFailure()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Strong_Passw0rd";
        var roleName = "Engineer";
        
        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync(roleName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApplicationRole() { Type = roleName });

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException());
        
        // Act
        var result = await _authService.RegisterAsync(email, password, roleName);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_WhenOperationCanceled_ShouldReturnResultFailure()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Strong_Passw0rd";
        var roleName = "Engineer";
        var cancellationToken = new CancellationToken(canceled: true);

        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync(roleName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _authService.RegisterAsync(email, password, roleName, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenRefreshSucceeded_ShouldReturnResultSuccess()
    {
        // Arrange
        var oldPlainJwtTokens = new PlainJwtTokensDto("oldAccessToken", "oldRefreshToken");
        var newJwtTokens = new JwtTokenDto()
        {
            AccessToken = "newAccessToken",
            RefreshTokenDto = new RefreshTokenDto(new RefreshToken(), "newRefreshToken")
        };

        _refreshTokenServiceMock
            .Setup(s => s.RefreshAsync(It.Is<string>(s => s == oldPlainJwtTokens.RefreshToken),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newJwtTokens);
        
        // Act
        var result = await _authService.RefreshTokensAsync(oldPlainJwtTokens);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(result.Value.AccessToken, newJwtTokens.AccessToken);
        Assert.Equal(result.Value.RefreshToken, newJwtTokens.RefreshTokenDto.PlainRefreshToken);
    }

    [Fact]
    public async Task RefreshAsync_WhenRefreshFailed_ShouldReturnResultFailure()
    {
        // Arrange
        var oldPlainJwtTokens = new PlainJwtTokensDto("oldAccessToken", "oldRefreshToken");
        
        _refreshTokenServiceMock
            .Setup(s => s.RefreshAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException());
        
        // Act
        var result = await _authService.RefreshTokensAsync(oldPlainJwtTokens);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task RefreshAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var oldPlainJwtTokens = new PlainJwtTokensDto("oldAccessToken", "oldRefreshToken");
        var cancellationToken = new CancellationToken(canceled: true);
        
        _refreshTokenServiceMock
            .Setup(s => s.RefreshAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _authService.RefreshTokensAsync(oldPlainJwtTokens, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task RefreshAsync_WhenAccessTokenIsNull_ShouldReturnResultSuccess()
    {
        // Arrange
        var oldPlainJwtTokens = new PlainJwtTokensDto(null, "oldRefreshToken");
        var newJwtTokens = new JwtTokenDto()
        {
            AccessToken = "newAccessToken",
            RefreshTokenDto = new RefreshTokenDto(new RefreshToken(), "newRefreshToken")
        };

        _refreshTokenServiceMock
            .Setup(s => s.RefreshAsync(It.Is<string>(s => s == oldPlainJwtTokens.RefreshToken),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newJwtTokens);
        
        // Act
        var result = await _authService.RefreshTokensAsync(oldPlainJwtTokens);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(result.Value.AccessToken, newJwtTokens.AccessToken);
        Assert.Equal(result.Value.RefreshToken, newJwtTokens.RefreshTokenDto.PlainRefreshToken);
    }
}
