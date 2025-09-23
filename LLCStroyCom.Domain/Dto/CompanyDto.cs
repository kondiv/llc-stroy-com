namespace LLCStroyCom.Domain.Dto;

public class CompanyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public CompanyDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
