using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Dto;

public class ProjectPatchDto
{
    public string Name { get; set; }
    public string City { get; set; }
    public Status Status { get; set; }
}