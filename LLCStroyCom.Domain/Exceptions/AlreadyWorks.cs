namespace LLCStroyCom.Domain.Exceptions;

public class AlreadyWorks : Exception
{
    private AlreadyWorks(string message) : base(message)
    {
        
    }

    public static AlreadyWorks InCompany(Guid companyId)
    {
        return new AlreadyWorks("Already works in company: " + companyId);
    }
}