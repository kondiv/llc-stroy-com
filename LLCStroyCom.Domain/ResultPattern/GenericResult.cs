namespace LLCStroyCom.Domain.ResultPattern;

public class Result<T> : ResultPattern.Result
{
    private readonly T? _value;
    public T Value => Succeeded ? _value! : throw new InvalidOperationException("Cannot access value of Result.Failure");

    private Result(T? value, bool succeeded, IEnumerable<Error> errors)
        : base(succeeded, errors)
    {
        if (succeeded && value is null)
        {
            throw new ArgumentNullException(nameof(value), "Cannot create Result.Success without value");
        }
        
        _value = value;
    }

    private Result(T? value, bool succeeded, IEnumerable<Error> errors, Exception innerException)
        : base(succeeded, errors, innerException)
    {
        if (succeeded && value is null)
        {
            throw new ArgumentNullException(nameof(value), "Cannot create Result.Success without value");
        }
        
        _value = value;
    }
    
    public new static Result<T> Failure(IEnumerable<Error> errors) => new Result<T>(default, false, errors);
    public new static Result<T> Failure(Error error) => new Result<T>(default, false, [error]);
    
    public new static Result<T> Failure(IEnumerable<Error> errors, Exception innerException) 
        => new Result<T>(default, false, errors, innerException);
    public static Result<T> Success(T value) => new Result<T>(value, true, []);
}