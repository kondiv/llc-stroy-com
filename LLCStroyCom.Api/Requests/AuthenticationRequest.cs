using System.ComponentModel.DataAnnotations;

namespace LLCStroyCom.Api.Requests;

public sealed class AuthenticationRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [MinLength(6, ErrorMessage = "Email must be at least 6 characters long")]
    [MaxLength(128, ErrorMessage = "Email must not exceed 128 characters")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [MaxLength(30, ErrorMessage = "Password must not exceed 30 characters")]
    public string Password { get; set; } = null!;
}