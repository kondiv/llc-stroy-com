namespace LLCStroyCom.Domain.Specifications.Companies;

public class CompanyFilter
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string OrderBy { get; set; } = "name";
    public bool OrderByDescending { get; set; } = false;
}