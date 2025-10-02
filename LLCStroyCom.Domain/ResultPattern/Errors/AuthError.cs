namespace LLCStroyCom.Domain.ResultPattern.Errors;

public class AuthError : Error
{
    public AuthError(string message)
        : base(message)
    {
        ErrorCode = ErrorCode.AuthProblem;
    }
}