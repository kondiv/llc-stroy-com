using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Models.Filters.Project;

public class ProjectFilter
{
    public Guid? CompanyId { get; set; }
    public Status? Status { get; set; }
    public string? City { get; set; }
    public string OrderBy { get; set; } = "name";
    public bool Descending { get; set; } = false;
}