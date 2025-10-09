namespace LLCStroyCom.Domain.Dto;

public class EmployeeDto
{
    public string Name { get; set; } = null!;
    public EmployeeDto(string name)
    {
        Name = name;
    }
}