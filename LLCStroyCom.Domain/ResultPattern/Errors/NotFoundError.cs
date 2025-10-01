namespace LLCStroyCom.Domain.ResultPattern.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message)
        : base(message)
    {
        ErrorCode = ErrorCode.NotFound;
    }
}