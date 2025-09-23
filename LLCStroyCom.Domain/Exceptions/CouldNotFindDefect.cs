namespace LLCStroyCom.Domain.Exceptions;

public class CouldNotFindDefect : Exception
{
    private CouldNotFindDefect(string message)
        : base(message)
    {
        
    }

    public static CouldNotFindDefect WithId(Guid id)
    {
        return new CouldNotFindDefect($"Defect with Id: {id} could not be found");
    }
}