namespace LLCStroyCom.Domain.Exceptions;

public class CouldNotFindProject : Exception
{
    private CouldNotFindProject(string message)
        : base(message)
    {
        
    }

    public static CouldNotFindProject WithId(Guid id)
    {
        return new CouldNotFindProject($"Project with id: {id} could not be found");
    }
}