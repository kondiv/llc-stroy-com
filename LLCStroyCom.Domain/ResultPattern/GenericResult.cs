using LLCStroyCom.Domain.ResultPattern.Errors;

namespace LLCStroyCom.Domain.ResultPattern;

public class Result<T> : ResultPattern.Result
{
    private readonly T? _value;
    public T Value => Succeeded ? _value! : throw new InvalidOperationException("Cannot access value of Result.Failure");

    private Result(T? value, bool succeeded, Error error)
        : base(succeeded, error)
    {
        if (succeeded && value is null)
        {
            throw new ArgumentNullException(nameof(value), "Cannot create Result.Success without value");
        }
        
        _value = value;
    }

    private Result(T? value, bool succeeded, Error error, Exception innerException)
        : base(succeeded, error, innerException)
    {
        if (succeeded && value is null)
        {
            throw new ArgumentNullException(nameof(value), "Cannot create Result.Success without value");
        }
        
        _value = value;
    }
    
    public new static Result<T> Failure(Error error) => new Result<T>(default, false, error);
    
    public new static Result<T> Failure(Error error, Exception innerException) 
        => new Result<T>(default, false, error, innerException);
    public static Result<T> Success(T value) => new Result<T>(value, true, null);
}