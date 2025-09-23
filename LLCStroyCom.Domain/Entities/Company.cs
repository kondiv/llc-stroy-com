namespace LLCStroyCom.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Project> Projects { get; set; } = [];
    
    public virtual ICollection<ApplicationUser> Employees { get; set; } = [];
}