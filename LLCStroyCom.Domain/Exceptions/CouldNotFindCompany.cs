namespace LLCStroyCom.Domain.Exceptions;

public class CouldNotFindCompany : Exception
{
    private CouldNotFindCompany(string message)
        : base(message)
    {
        
    }

    public static CouldNotFindCompany WithId(Guid id)
    {
        return new CouldNotFindCompany($"Company with Id: {id} could not be found");
    }
}