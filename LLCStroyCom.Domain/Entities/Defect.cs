using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Entities;

public class Defect
{
    public Guid Id { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid? ChiefEngineerId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public Status Status { get; set; }
    
    public string Description { get; set; } = null!;
    
    public ApplicationUser? ChiefEngineer { get; set; }
    
    public Project Project { get; set; } = null!;
}