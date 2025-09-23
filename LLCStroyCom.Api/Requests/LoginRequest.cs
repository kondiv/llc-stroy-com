using System.ComponentModel.DataAnnotations;

namespace LLCStroyCom.Api.Requests;

public class LoginRequest
{
   [Required(ErrorMessage = "Email is required")]
   [EmailAddress(ErrorMessage = "Invalid format")]
   public string Email { get; set; } = null!;

   [Required(ErrorMessage = "Password is required")]
   public string Password { get; set; } = null!;
}