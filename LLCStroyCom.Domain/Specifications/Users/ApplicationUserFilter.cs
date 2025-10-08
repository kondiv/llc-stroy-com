namespace LLCStroyCom.Domain.Specifications.Users;

public class ApplicationUserFilter
{
    public string? Name { get; set; }
    public int? RoleId { get; set; }
    public string OrderBy { get; set; } = "name";
    public bool OrderByDescending { get; set; } = false;
}