using LLCStroyCom.Application.Validators.Auth;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Response;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;

namespace LLCStroyCom.Application.Services;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AuthenticationDataValidator _authenticationDataValidator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        AuthenticationDataValidator authenticationDataValidator,
        ILogger<AuthService> logger)
    {
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _authenticationDataValidator = authenticationDataValidator;
        _logger = logger;
    }

    public async Task<Result> RegisterAsync(string email, string password, string roleName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering new user");
        
        var validationResult = await ValidateAuthenticationDataAsync(email, password);
        if (!validationResult.Succeeded)
        {
            _logger.LogWarning($"Invalid email or password: {validationResult.Errors}");
            return validationResult;
        }

        try
        {
            var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);

            var newUser = new ApplicationUser()
            {
                Email = email,
                HashPassword = _passwordHasher.HashPassword(password),
                Role = role
            };

            var userId = await _userRepository.CreateAsync(newUser, cancellationToken);

            _logger.LogInformation("Created user with id: {userId}", userId);

            return Result.Success();
        }
        catch (RoleCouldNotBeFound e)
        {
            _logger.LogError(e, "Role \"{roleName}\" was not found. User was not created", roleName);
            return Result.Failure(new Error(e.Message, "AuthError"));
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, "User was not created");
            return Result.Failure(new Error(e.Message, "AuthError"));
        }
    }

    public async Task<Result<PlainJwtTokensDto>>
        LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started to login user");
        
        var validationResult = await ValidateAuthenticationDataAsync(email, password);

        if (!validationResult.Succeeded)
        {
            _logger.LogWarning($"Invalid email or password: {validationResult.Errors}");
            return Result<PlainJwtTokensDto>.Failure();
        }

        try
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (!_passwordHasher.VerifyPassword(password, user.HashPassword))
            {
                _logger.LogWarning("Invalid email or password");
                return Result<PlainJwtTokensDto>.Failure(
                    new Error("Invalid email or password", "InvalidCredentials"));
            }

            var tokens = await _tokenService.CreateTokensAsync(user);

            await _userRepository.AssignNewAndRevokeOldRefreshTokenAsync(
                user.Id, tokens.RefreshTokenDto.RefreshTokenEntity, cancellationToken);

            return Result<PlainJwtTokensDto>.Success(
                new PlainJwtTokensDto(tokens.AccessToken, tokens.RefreshTokenDto.PlainRefreshToken));
        }
        catch (UserCouldNotBeFound e)
        {
            _logger.LogError(e, "User with email \"{email}\" could not be found ", email);
            return Result<PlainJwtTokensDto>.Failure(new Error(e.Message, "AuthError"));
        }
    }

    // TODO Write tests
    public async Task<Result<PlainJwtTokensDto>> 
        RefreshTokensAsync(PlainJwtTokensDto tokens, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started to refresh user's tokens");

        try
        {
            var refreshedTokens = await _refreshTokenService.RefreshAsync(tokens.RefreshToken,
                cancellationToken);

            return Result<PlainJwtTokensDto>.Success(
                new PlainJwtTokensDto(refreshedTokens.AccessToken,
                    refreshedTokens.RefreshTokenDto.PlainRefreshToken));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result<PlainJwtTokensDto>.Failure();
        }
    }

    private async Task<Result> ValidateAuthenticationDataAsync(string email, string password)
    {
        var validationResult = await _authenticationDataValidator.ValidateAsync(
            new RegistrationDataValidationDto(email, password));

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors
                .Select(vf => new Error(vf.ErrorMessage, vf.ErrorCode))
                .ToList());
        }

        return Result.Success();
    }
}