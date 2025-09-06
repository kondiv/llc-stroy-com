namespace LLCStroyCom.Domain.Exceptions;

public class UserCouldNotBeFound : Exception
{
    public UserCouldNotBeFound(string message)
        : base(message)
    {
        
    }
    
    public static UserCouldNotBeFound WithId(Guid id)
    {
        return new UserCouldNotBeFound($"Could not find user with id: {id}");
    }
}