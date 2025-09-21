using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Services;

namespace LLCStroyCom.Tests.Services;

public class BCryptPasswordHasherTests
{
    private readonly IPasswordHasher _passwordHasher;

    public BCryptPasswordHasherTests()
    {
        _passwordHasher = new BCryptPasswordHasher();
    }

    [Fact]
    public void HashPassword_WhenPasswordIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? password = null;
        
        // Act
        var act = () => _passwordHasher.HashPassword(password!);
        
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void HashPassword_WhenPasswordIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var password = string.Empty;
        
        // Act
        var act = () => _passwordHasher.HashPassword(password);
        
        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void HashPassword_WhenPasswordIsWhiteSpace_ShouldThrowArgumentException()
    {
        // Arrange
        var password = "     ";
        
        // Act
        var act = () => _passwordHasher.HashPassword(password);
        
        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void HashPassword_WhenPasswordIsValid_ShouldReturnValidHashPassword()
    {
        // Arrange
        var password = "Passw0rd_";
        
        // Act
        var passwordHash = _passwordHasher.HashPassword(password);
        
        // Assert
        Assert.True(_passwordHasher.VerifyPassword(password, passwordHash));
    }

    [Fact]
    public void HashPassword_WhenPasswordsAreTheSame_ShouldProduceDifferentHashes()
    {
        // Arrange
        var firstPassword = "Passw0rd_";
        var secondPassword = firstPassword;
        
        // Act
        var firstPasswordHash = _passwordHasher.HashPassword(firstPassword);
        var secondPasswordHash = _passwordHasher.HashPassword(secondPassword);
        
        // Assert
        Assert.NotEqual(firstPasswordHash, secondPasswordHash);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordIsNull_ShouldReturnFalse()
    {
        // Arrange
        string? password = null;
        var passwordHash = "passwordHash";
        
        // Act
        var result =  _passwordHasher.VerifyPassword(password!, passwordHash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var password = string.Empty;
        var passwordHash = "passwordHash";
        
        // Act
        var result =  _passwordHasher.VerifyPassword(password, passwordHash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordIsWhiteSpace_ShouldReturnFalse()
    {
        // Arrange
        var password = "    ";
        var passwordHash = "passwordHash";
        
        // Act
        var result =  _passwordHasher.VerifyPassword(password, passwordHash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordHashIsNull_ShouldReturnFalse()
    {
        // Arrange
        var password = "Passw0rd_";
        string? passwordHash = null;
        
        // Act
        var result =  _passwordHasher.VerifyPassword(password, passwordHash!);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordHashIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var password = "Passw0rd_";
        var passwordHash = string.Empty;
        
        // Act
        var result =  _passwordHasher.VerifyPassword(password, passwordHash);
        
        // Assert
        Assert.False(result);
        
    }

    [Fact]
    public void VerifyPassword_WhenPasswordHashIsWhiteSpace_ShouldReturnFalse()
    {
        // Arrange
        var password = "Passw0rd_";
        var passwordHash = "    ";
        
        // Act
        var result =  _passwordHasher.VerifyPassword(password, passwordHash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordHashNotFromThisPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "Passw0rd_";
        var passwordHash = _passwordHasher.HashPassword("randomPassword");
        
        // Act
        var result = _passwordHasher.VerifyPassword(password, passwordHash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordHashIfFromThisPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "Passw0rd_";
        var passwordHash = _passwordHasher.HashPassword(password);
        
        // Act
        var result  = _passwordHasher.VerifyPassword(password, passwordHash);
        
        // Assert
        Assert.True(result);
    }
}