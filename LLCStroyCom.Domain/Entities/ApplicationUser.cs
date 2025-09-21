namespace LLCStroyCom.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }
    
    public string Email { get; set; } = null!;
    
    public string HashPassword { get; set; } = null!;
    
    public int RoleId { get; set; }
    
    public virtual ICollection<Defect> Defects { get; set; } = [];
    
    public virtual ApplicationRole Role { get; set; } = null!;
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}