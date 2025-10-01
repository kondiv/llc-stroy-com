using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string City { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public Status Status { get; set; }
    
    public Guid CompanyId { get; set; }
    
    public virtual Company Company { get; set; } = null!;
    
    public virtual ICollection<Defect> Defects { get; set; } = [];
}