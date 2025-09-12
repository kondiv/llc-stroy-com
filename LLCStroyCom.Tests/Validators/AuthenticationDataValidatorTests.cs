using LLCStroyCom.Application.Validators.Auth;
using LLCStroyCom.Domain.Dto;

namespace LLCStroyCom.Tests.Validators;

public class AuthenticationDataValidatorTests
{
    private readonly AuthenticationDataValidator _validator = new AuthenticationDataValidator();

    [Fact]
    public async Task ValidateAsync_WhenEverythingIsValid_ShouldReturnValidationResultIsValid()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WhenEmailIsNull_ShouldReturnValidationResultIsInvalid_WithMessageEmailIsRequired()
    {
        // Arrange
        string? email = null;
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ValidateAsync_WhenEmailIsEmpty_ShouldReturnValidationResultIsInvalid_WithMessageEmailIsRequired()
    {
        // Arrange
        var email = string.Empty;
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task 
        ValidateAsync_WhenEmailIsWhiteSpace_ShouldReturnValidationResultIsInvalid_WithMessageEmailIsRequired()
    {
        // Arrange
        var email = " ";
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenEmailIsTooShort_ShouldReturnValidationResultIsInvalid_WithMessageEmailMustBeAtLeast6CharactersLong()
    {
        // Arrange
        var email = "e@e.r";
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email must be at least 6 characters long", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenEmailIsTooLong_ShouldReturnValidationResultIsInvalid_WithMessageEmailMustNotExceed100Characters()
    {
        // Arrange
        var email = new string('a', 90) + '@' + new string('a', 8) + ".ru";
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email must not exceed 100 characters", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenEmailIsInvalidFormat_ShouldReturnValidationResultIsInvalid_WithMessageEmailIsInvalid()
    {
        // Arrange
        var email = "notemail";
        var password = "Password_1";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email is invalid", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenPasswordIsNull_ShouldReturnValidationResultIsInvalid_WithMessagePasswordIsRequired()
    {
        // Arrange
        var email = "email@email.com";
        string? password = null;
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenPasswordIsEmpty_ShouldReturnValidationResultIsInvalid_WithMessagePasswordIsRequired()
    {
        // Arrange
        var email = "email@email.com";
        var password = string.Empty;
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenPasswordIsWhiteSpace_ShouldReturnValidationResultIsInvalid_WithMessagePasswordIsRequired()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "    ";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenPasswordIsTooShort_ShouldReturnValidationResultIsInvalid_WithMessagePasswordMustBeAtLeast6CharactersLong()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Asd1_";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password must be at least 6 characters long", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenPasswordIsTooLong_ShouldReturnValidationResultIsInvalid_WithMessagePasswordMustNotExceed30Characters()
    {
        // Arrange
        var email = "email@email.ru";
        var password = "Asd_" + new string('1', 30);
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password must not exceed 30 characters", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task
        ValidateAsync_WhenPasswordIsWeak_ShouldReturnValidationResultIsInvalid_WithMessagePasswordIsTooWeak()
    {
        // Arrange
        var email = "email@email.com";
        var password = "iAmWeakPassword";
        
        // Act
        var result = await _validator.ValidateAsync(new RegistrationDataValidationDto(email, password));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password is too weak", result.Errors[0].ErrorMessage);
    }
}