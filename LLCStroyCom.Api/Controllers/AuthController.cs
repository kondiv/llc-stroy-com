using LLCStroyCom.Api.Requests;
using LLCStroyCom.Domain.Configs;
using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IAuthService authService, IOptions<JwtSettings> jwtSettings)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("register/engineer")]
    public async Task<ActionResult> 
        RegisterEngineerAsync([FromBody] AuthenticationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authenticationResult = await _authService.RegisterAsync (
            request.Email, request.Password, RoleType.Engineer, cancellationToken);

        if (!authenticationResult.Succeeded)
        {
            return BadRequest(authenticationResult.Errors);
        }

        return Created();
    }

    [HttpPost("register/manager")]
    public async Task<ActionResult>
        RegisterManagerAsync([FromBody] AuthenticationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var authenticationResult = await _authService.RegisterAsync(
            request.Email, request.Password, RoleType.Manager, cancellationToken);
        
        if (!authenticationResult.Succeeded)
        {
            return BadRequest(authenticationResult.Errors);
        }

        return Created();
    }

    [HttpPost("register/observer")]
    public async Task<ActionResult>
        RegisterObserverAsync([FromBody] AuthenticationRequest request, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var authenticationResult = await _authService.RegisterAsync(
            request.Email, request.Password, RoleType.Observer, cancellationToken);
        
        if(!authenticationResult.Succeeded)
        {
            return BadRequest(authenticationResult.Errors);
        }

        return Created();
    }

    [HttpPost("login")]
    public async Task<ActionResult>
        LoginAsync([FromBody] AuthenticationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authenticationResult = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (!authenticationResult.Succeeded)
        {
            return BadRequest(authenticationResult.Errors);
        }

        if (authenticationResult.Value is null)
        {
            return BadRequest("Critical error");
        }
        
        SetTokensInsideCookie(HttpContext, authenticationResult.Value);

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshAsync()
    {
        HttpContext.Request.Cookies.TryGetValue("access_token", out var accessToken);
        HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

        var plainJwtTokensDto = new PlainJwtTokensDto(accessToken ?? string.Empty, refreshToken ?? string.Empty);

        var tokens = await _authService.RefreshTokensAsync(plainJwtTokensDto);

        if (tokens.Value is null)
        {
            return BadRequest();
        }
        
        SetTokensInsideCookie(HttpContext, tokens.Value);

        return Ok();
    }

    private void SetTokensInsideCookie(HttpContext httpContext, PlainJwtTokensDto tokens)
    {
        HttpContext.Response.Cookies.Append("access_token", tokens.AccessToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        
        HttpContext.Response.Cookies.Append("refresh_token", tokens.RefreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }
}