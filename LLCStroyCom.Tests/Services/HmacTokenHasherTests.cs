using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Services;

namespace LLCStroyCom.Tests.Services;

public class HmacTokenHasherTests
{
    private readonly ITokenHasher _tokenHasher;

    public HmacTokenHasherTests()
    {
        _tokenHasher = new HmacTokenHasher("very_secret_secret");
    }

    [Fact]
    public void HashToken_WhenTokenIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? token = null;
        
        // Act
        var act = () =>  _tokenHasher.HashToken(token);
        
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void HashToken_WhenTokenIsEmpty_ThrowsArgumentException()
    { 
        // Arrange
        var token = string.Empty;
        
        // Act
        var act = () =>  _tokenHasher.HashToken(token);
        
        // Assert
        Assert.Throws<ArgumentException>(act);
    }
    
    [Fact]
    public void HashToken_WhenStringIsWhiteSpace_ThrowsArgumentException()
    {
        // Arrange
        var token = "   ";
        
        // Act
        var act = () => _tokenHasher.HashToken(token);
        
        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void HashToken_WhenTokensAreTheSame_ShouldProduceSameHashedTokens()
    {
        // Arrange
        var firstToken = "tokentoken";
        var secondToken = "tokentoken";
        
        // Act
        var firstTokenHash = _tokenHasher.HashToken(firstToken);
        var secondTokenHash = _tokenHasher.HashToken(secondToken);
        
        // Assert
        Assert.Equal(firstTokenHash, secondTokenHash);
    }
}