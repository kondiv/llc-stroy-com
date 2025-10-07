using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Dto;

public class DefectDto
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public string Description { get; init; }
    public Status Status { get; init; }
    public ProjectDto Project { get; init; }
    public ChiefEngineerDto? ChiefEngineer { get; init; }

    public DefectDto(Guid id, string name, string description, Status status, ProjectDto project, ChiefEngineerDto? chiefEngineer)
    {
        Id = id;
        Name = name;
        Description = description;
        Status = status;
        Project = project;
        ChiefEngineer = chiefEngineer;
    }
}