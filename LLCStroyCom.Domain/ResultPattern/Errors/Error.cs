namespace LLCStroyCom.Domain.ResultPattern.Errors;

public class Error
{
    public string Message { get; }
    public ErrorCode ErrorCode { get; protected init; }

    protected Error(string message)
    {
        Message = message;
    }

    public Error(string message, ErrorCode errorCode)
    {
        Message = message;
        ErrorCode = errorCode;
    }
}