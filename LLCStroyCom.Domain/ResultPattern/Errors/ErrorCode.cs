namespace LLCStroyCom.Domain.ResultPattern.Errors;

public enum ErrorCode
{
    DbUpdate,
    NotFound,
    DbUpdateConcurrency,
    Conflict,
    AlreadyExists,
    AuthProblem,
    Validation
}