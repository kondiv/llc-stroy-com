namespace LLCStroyCom.Domain.Exceptions;

public class ProjectCouldNotBeFound : Exception
{
    private ProjectCouldNotBeFound(string message)
        : base(message)
    {
        
    }

    public static ProjectCouldNotBeFound WithId(Guid id)
    {
        return new ProjectCouldNotBeFound($"Project with id: {id} could not be found");
    }
}