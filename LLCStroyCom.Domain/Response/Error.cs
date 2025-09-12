namespace LLCStroyCom.Domain.Response;

public struct Error
{
    private readonly string _message;
    private readonly string _errorCode;

    public Error(string message, string errorCode)
    {
        _message = message;
        _errorCode = errorCode;
    }
}