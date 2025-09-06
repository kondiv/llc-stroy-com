namespace LLCStroyCom.Domain.Entities;

public class ApplicationRole
{
    public int Id { get; set; }
    public string Type { get; set; }
    public ICollection<ApplicationUser> Users { get; set; }
}