namespace LLCStroyCom.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string HashPassword { get; set; } = null!;
    
    public Guid? CompanyId { get; set; }

    public virtual Company? Company { get; set; }
    
    public int RoleId { get; set; }

    public virtual ApplicationRole Role { get; set; } = null!;

    public virtual ICollection<Defect> Defects { get; set; } = [];
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}