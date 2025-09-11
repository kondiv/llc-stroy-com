using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Api.Requests;
using LLCStroyCom.Domain.Configs;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Response;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _authController;
    
    public AuthControllerTests()
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
        jwtSettingsMock.Setup(o => o.Value).Returns(_jwtSettings);
        
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object, jwtSettingsMock.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task RegisterEngineerAsync_WhenAuthServiceReturnsSuccess_ShouldReturnCreated()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "email@email.ru",
            Password = "password"
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                CancellationToken.None))
            .ReturnsAsync(Result.Success);
        
        // Act
        var actionResult = await _authController.RegisterEngineerAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        var result = actionResult as CreatedResult;
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(actionResult);
    }

    [Fact]
    public async Task RegisterEngineerAsync_WhenAuthServiceReturnsError_ShouldReturnBadRequest()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "email@email.ru",
            Password = "password"
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                CancellationToken.None))
            .ReturnsAsync(Result.Failure());
        
        // Act
        var actionResult = await _authController.RegisterEngineerAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "email@email.ru",
            Password = "pass"
        };
        _authController.ModelState.AddModelError("Password", "Password is invalid");
        
        // Act
        var actionResult = await _authController.RegisterEngineerAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "email",
            Password = "Password_1234"
        };
        _authController.ModelState.AddModelError("Email", "Email is invalid");
        
        // Act
        var actionResult = await _authController.RegisterEngineerAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task RegisterManagerAsync_WhenAuthServiceReturnsSuccess_ShouldReturnCreated()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "manager@email.com",
            Password = "Password_1234"
        };
        
        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                CancellationToken.None))
            .ReturnsAsync(Result.Success);
        
        // Act
        var actionResult = await _authController.RegisterManagerAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<CreatedResult>(actionResult);
    }

    [Fact]
    public async Task RegisterManagerAsync_WhenAuthServiceReturnsFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "manager@email.ru",
            Password = "Password_1234"
        };
        
        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                CancellationToken.None))
            .ReturnsAsync(Result.Failure());
        
        // Act
        var actionResult = await _authController.RegisterManagerAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task RegisterObserverAsync_WhenAuthServiceReturnsSuccess_ShouldReturnCreated()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "observer@email.com",
            Password = "Password_1234"
        };
        
        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                CancellationToken.None))
            .ReturnsAsync(Result.Success);
        
        // Act
        var actionResult = await _authController.RegisterObserverAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<CreatedResult>(actionResult);
    }

    [Fact]
    public async Task RegisterObserverAsync_WhenAuthServiceReturnsFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var authenticationRequest = new AuthenticationRequest()
        {
            Email = "observer@email.ru",
            Password = "Password_1234"
        };
        
        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                CancellationToken.None))
            .ReturnsAsync(Result.Failure());
        
        // Act
        var actionResult = await _authController.RegisterObserverAsync(authenticationRequest, CancellationToken.None);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task LoginAsync_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AuthenticationRequest();
        _authController.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_WhenLoginFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AuthenticationRequest { Email = "test@email.com", Password = "123456" };

        var failedResult = Result<PlainJwtTokensDto>.Failure();

        _authServiceMock
            .Setup(s => s.LoginAsync(request.Email, request.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResult);

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_WhenValueIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AuthenticationRequest { Email = "test@email.com", Password = "123456" };

        var resultWithoutValue = Result<PlainJwtTokensDto>.Success(null);

        _authServiceMock
            .Setup(s => s.LoginAsync(request.Email, request.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultWithoutValue);

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_WhenLoginSucceeds_ShouldReturnOk()
    {
        // Arrange
        var request = new AuthenticationRequest { Email = "test@email.com", Password = "123456" };

        var tokens = new PlainJwtTokensDto("access_token_value", "refresh_token_value");

        var successResult = Result<PlainJwtTokensDto>.Success(tokens);

        _authServiceMock
            .Setup(s => s.LoginAsync(request.Email, request.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task RefreshAsync_WhenNoCookies_ShouldReturnBadRequest()
    {
        // Arrange
        // Куки не добавляем

        var failedResult = Result<PlainJwtTokensDto>.Failure();

        _authServiceMock
            .Setup(s => s.RefreshTokensAsync(It.IsAny<PlainJwtTokensDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResult);

        // Act
        var result = await _authController.RefreshAsync();

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}