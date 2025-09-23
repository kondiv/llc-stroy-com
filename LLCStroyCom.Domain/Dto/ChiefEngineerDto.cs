namespace LLCStroyCom.Domain.Dto;

public class ChiefEngineerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public ChiefEngineerDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}