using LLCStroyCom.Domain.ResultPattern.Errors;

namespace LLCStroyCom.Domain.ResultPattern;

public class Result
{
    private readonly Exception? _innerException;
    private readonly Error? _error;

    public Error Error =>
        IsFailure ? _error! : throw new InvalidOperationException("Cannot access Error in ResultSuccess");

    public Exception InnerException => IsFailure
        ? _innerException!
        : throw new InvalidOperationException("Cannot access Exception in Succeeded Result");

    public bool Succeeded { get; }
    
    public bool IsFailure => !Succeeded;

    protected Result(bool succeeded, Error? error)
    {
        if (!succeeded && error is null)
        {
            throw new ArgumentException("Cannot create Result Failure without Error");
        }

        Succeeded = succeeded;
        _error = error;
    }

    protected Result(bool succeeded, Error? error, Exception? innerException)
    {
        if (!succeeded && error is null && innerException is null)
        {
            throw new ArgumentException("Cannot create Result Failure without Error or Exception");
        }

        Succeeded = succeeded;
        _error = error;
        _innerException = innerException;
    }

    public static Result Success() => new Result(true, null);
    public static Result Failure(Error error) => new Result(false, error);
    public static Result Failure(Error error, Exception innerException) => new Result(false, error, innerException);
}