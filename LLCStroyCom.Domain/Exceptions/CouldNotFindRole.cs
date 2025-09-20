namespace LLCStroyCom.Domain.Exceptions;

public class CouldNotFindRole : Exception
{
    private CouldNotFindRole(string message) : base(message)
    {
        
    }

    public static CouldNotFindRole WithName(string name)
    {
        return new CouldNotFindRole($"Could not find role with name: {name}");
    }
}