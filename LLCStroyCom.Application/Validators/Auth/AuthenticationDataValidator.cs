using System.Text.RegularExpressions;
using FluentValidation;
using LLCStroyCom.Domain.Dto;

namespace LLCStroyCom.Application.Validators.Auth;

public sealed class AuthenticationDataValidator : AbstractValidator<RegistrationDataValidationDto>
{
    public AuthenticationDataValidator()
    {
        RuleFor(dto => dto.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .Must(NotBeWhiteSpace).WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid")
            .MinimumLength(6).WithMessage("Email must be at least 6 characters long")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(dto => dto.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters")
            .Must(BeStrong).WithMessage("Password is too weak");
    }

    private bool BeStrong(string password)
    {
        var pattern = @"^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?])(?=.*[0-9]).{6,30}$";
        var regex = new Regex(pattern);
        return regex.IsMatch(password);
    }

    private bool NotBeWhiteSpace(string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }
}