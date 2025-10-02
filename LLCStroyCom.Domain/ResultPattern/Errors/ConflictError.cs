namespace LLCStroyCom.Domain.ResultPattern.Errors;

public class ConflictError : Error
{
    public ConflictError(string message)
        : base(message)
    {
        ErrorCode = ErrorCode.Conflict;
    }
}