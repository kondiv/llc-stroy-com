namespace LLCStroyCom.Domain.ResultPattern.Errors;

public class AlreadyExistsError : Error
{
    public AlreadyExistsError(string message)
        : base(message)
    {
        ErrorCode = ErrorCode.AlreadyExists;
    }
}