namespace LLCStroyCom.Domain.ResultPattern;

public struct Error
{
    public string Message { get; }
    public ErrorCode ErrorCode { get; }

    public Error(string message, ErrorCode errorCode)
    {
        Message = message;
        ErrorCode = errorCode;
    }
    
    public static Error NotFound(string message) => new Error(message, ErrorCode.NotFound);
    public static Error Conflict(string message) => new Error(message, ErrorCode.Conflict);
    public static Error AlreadyInUse(string message) => new Error(message, ErrorCode.AlreadyInUse);
    public static Error Auth(string message) => new Error(message, ErrorCode.AuthError);
    public static Error Validation(string message) => new Error(message, ErrorCode.ValidationError);
}