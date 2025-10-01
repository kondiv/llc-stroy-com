using System.Diagnostics;
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
    public void HashPassword_ShouldMeasurePerformance()
    {
        // Arrange
        var password = "TestPassword123!";
        var iterations = 5;
        var executionTimes = new List<long>();

        // Act & Measure
        for (int i = 0; i < iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var hashedPassword = _passwordHasher.HashPassword(password);
            stopwatch.Stop();
                
            executionTimes.Add(stopwatch.ElapsedMilliseconds);
                
            // Basic validation
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
        }

        // Assert & Report
        var averageTime = executionTimes.Average();
        var minTime = executionTimes.Min();
        var maxTime = executionTimes.Max();

        Console.WriteLine($"=== Password Hashing Performance ===");
        Console.WriteLine($"Iterations: {iterations}");
        Console.WriteLine($"Average time: {averageTime:F2}ms");
        Console.WriteLine($"Min time: {minTime}ms");
        Console.WriteLine($"Max time: {maxTime}ms");
        Console.WriteLine($"All times: {string.Join("ms, ", executionTimes)}ms");
            
        // Performance assertion (adjust based on your expectations)
        Assert.True(averageTime < 100, $"Average time {averageTime}ms should be less than 100ms");
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