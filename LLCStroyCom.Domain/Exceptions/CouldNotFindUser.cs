namespace LLCStroyCom.Domain.Exceptions;

public class CouldNotFindUser : CouldNotFind
{
    private CouldNotFindUser(string message)
        : base(message)
    {
        
    }
    
    public static CouldNotFindUser WithId(Guid id)
    {
        return new CouldNotFindUser($"Could not find user with id: {id}");
    }

    public static CouldNotFindUser WithEmail(string email)
    {
        return new CouldNotFindUser($"Could not find user with email: {email}");
    }
}