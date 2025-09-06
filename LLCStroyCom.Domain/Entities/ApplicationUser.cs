namespace LLCStroyCom.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string HashPassword { get; set; }
    public int RoleId { get; set; }
    public virtual ApplicationRole Role { get; set; }
}