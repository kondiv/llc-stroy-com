namespace LLCStroyCom.Domain.ResultPattern;

public class Result
{
    private readonly List<Error> _errors;

    private readonly Exception? _innerException;

    public IReadOnlyList<Error> Errors => _errors;

    public Exception InnerException => IsFailure
        ? _innerException!
        : throw new InvalidOperationException("Cannot access Exception in Succeeded Result");

    public bool Succeeded { get; }
    
    public bool IsFailure => !Succeeded;

    protected Result(bool succeeded, IEnumerable<Error> errors)
    {
        var errorList = errors.ToList();

        switch (succeeded)
        {
            case false when errorList.Count == 0:
                throw new ArgumentException("Cannot create Failure Result without errors");
            case true when errorList.Count > 0:
                throw new ArgumentException("Cannot create Success Result with errors");
        }

        Succeeded = succeeded;
        _errors = errorList;
    }

    protected Result(bool succeeded, IEnumerable<Error> errors, Exception? innerException)
    {
        var errorList = errors.ToList();

        switch (succeeded)
        {
            case false when errorList.Count == 0:
                throw new ArgumentException("Cannot create Failure Result without errors");
            case true when errorList.Count > 0:
                throw new ArgumentException("Cannot create Success Result with errors");
            case true when innerException is not null:
                throw new ArgumentException("Cannot create Success Result with Inner Exception");
        }

        Succeeded = succeeded;
        _errors = errorList;
        _innerException = innerException;
    }

    public static Result Success() => new Result(true, []);
    public static Result Failure(IEnumerable<Error> errors) => new Result(false, errors);
    public static Result Failure(Error error) => new Result(false, [error]);
    public static Result Failure(IEnumerable<Error> errors, Exception innerException) => new Result(false, errors, innerException);
}