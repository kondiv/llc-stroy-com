namespace LLCStroyCom.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    
    public Guid CompanyId { get; set; }
    public int StatusId { get; set; }

    public virtual Status Status { get; set; } = null!;
    public virtual Company Company { get; set; } = null!;
}