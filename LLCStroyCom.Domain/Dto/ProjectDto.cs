using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Dto;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public string City { get; init; }
    public Guid CompanyId { get; init; }
    public Status Status { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public ProjectDto(Guid id, string name, string city, Guid companyId, Status status, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        City = city;
        CompanyId = companyId;
        Status = status;
        CreatedAt = createdAt;
    }
}