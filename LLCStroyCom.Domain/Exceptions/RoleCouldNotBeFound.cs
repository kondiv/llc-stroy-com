namespace LLCStroyCom.Domain.Exceptions;

public class RoleCouldNotBeFound : Exception
{
    private RoleCouldNotBeFound(string message) : base(message)
    {
        
    }

    public static RoleCouldNotBeFound WithName(string name)
    {
        return new RoleCouldNotBeFound($"Could not find role with name: {name}");
    }
}