namespace LLCStroyCom.Domain.Dto;

public class UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Role { get; init; }
    public CompanyDto Company { get; init; }

    public UserDto(Guid id, string name, string role, CompanyDto company)
    {
        Id = id;
        Name = name;
        Role = role;
        Company = company;
    }
}