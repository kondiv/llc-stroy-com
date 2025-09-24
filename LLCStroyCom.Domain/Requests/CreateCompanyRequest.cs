namespace LLCStroyCom.Domain.Requests;

public class CreateCompanyRequest
{
    public string Name { get; init; }

    public CreateCompanyRequest(string name)
    {
        Name = name;
    }
}