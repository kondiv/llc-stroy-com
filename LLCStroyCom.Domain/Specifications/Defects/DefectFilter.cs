using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Specifications.Defects;

public class DefectFilter
{
    public string? Name { get; set; }
    public Status? Status { get; }
    public string OrderBy { get; } = "name";
    public bool OrderByDescending { get; } = false;
}