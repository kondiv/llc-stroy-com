using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class RefreshTokenRepositoryTests
{
    private const string Secret = "very_secret_secret";
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly ITokenHasher _tokenHasher = new HmacTokenHasher(Secret);

    public RefreshTokenRepositoryTests()
    {           
        var context = GetInMemoryDbContext();
        _tokenRepository = new RefreshTokenRepository(context, _tokenHasher);
    }
    
    private StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    [Fact]
    public async Task GetAsync_WhenStringIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? token = null;
        
        // Act
        var act = () => _tokenRepository.GetAsync(token);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task GetAsync_WhenStringIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var token = string.Empty;
        
        // Act
        var act = () => _tokenRepository.GetAsync(token);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task GetAsync_WhenStringIsWhitespace_ShouldThrowArgumentException()
    {
        // Arrange
        var token = "   ";
        
        // Act
        var act = () => _tokenRepository.GetAsync(token);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task GetAsync_WhenTokenDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var token = "asdfasdfasdf";
        
        // Act
        var result = await _tokenRepository.GetAsync(token);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WhenTokenExists_ShouldReturnToken()
    {
        // Arrange
        var plainToken = "asdfasdfasdf";
        var refreshTokenId = Guid.NewGuid();
        var refreshToken = new RefreshToken()
        {
            Id = refreshTokenId,
            TokenHash = _tokenHasher.HashToken(plainToken)
        };

        var context = GetInMemoryDbContext();
        var repository = new RefreshTokenRepository(context, _tokenHasher);
        
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
        
        // Act
        var result = await repository.GetAsync(plainToken);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(_tokenHasher.HashToken(plainToken), result.TokenHash);
    }
}