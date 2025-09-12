namespace LLCStroyCom.Domain.Entities;

public class Status
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}