namespace LLCStroyCom.Domain.Exceptions;

public class UserCouldNotBeFound : Exception
{
    private UserCouldNotBeFound(string message)
        : base(message)
    {
        
    }
    
    public static UserCouldNotBeFound WithId(Guid id)
    {
        return new UserCouldNotBeFound($"Could not find user with id: {id}");
    }

    public static UserCouldNotBeFound WithEmail(string email)
    {
        return new UserCouldNotBeFound($"Could not find user with email: {email}");
    }
}