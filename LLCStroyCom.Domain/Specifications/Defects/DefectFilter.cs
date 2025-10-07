using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Specifications.Defects;

public class DefectFilter
{
    public string? Name { get; set; }
    public Status? Status { get; set; }
    public string OrderBy { get; set; } = "created_at";
    public bool OrderByDescending { get; set; } = false;
}