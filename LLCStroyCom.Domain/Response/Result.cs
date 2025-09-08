namespace LLCStroyCom.Domain.Response;

public class Result
{
    private readonly List<Error> _errors;
    
    public bool Succeeded { get; private set; }
    
    public IReadOnlyCollection<Error> Errors => _errors;

    protected Result(bool succeeded, IEnumerable<Error> errors)
    {
        Succeeded = succeeded;
        _errors = errors.ToList();
    }

    public static Result Success() => new Result(true, []);
    
    public static Result Failure(params Error[] errors) => new Result(false, errors);
    public static Result Failure(List<Error> errors) => new Result(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T? value, bool succeeded, IEnumerable<Error> errors)
        : base(succeeded, errors)
    {
        Value = value;
    }
    
    public new static Result<T> Failure(params Error[] errors) => new Result<T>(default, false, errors);
    public static Result<T> Success(T? value) => new Result<T>(value, true, []);
}