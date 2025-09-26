namespace LLCStroyCom.Domain.Requests;

public class CompanyCreateRequest
{
    public string Name { get; init; }

    public CompanyCreateRequest(string name)
    {
        Name = name;
    }
}