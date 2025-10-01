namespace LLCStroyCom.Domain.ResultPattern.Errors;

public class DbUpdateConcurrencyError : Error
{
    public DbUpdateConcurrencyError(string message) 
        : base(message)
    {
        ErrorCode = ErrorCode.DbUpdateConcurrency;
    }
}