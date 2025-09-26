using LLCStroyCom.Domain.Exceptions;

namespace LLCStroyCom.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string HashPassword { get; set; } = null!;
    
    public Guid? CompanyId { get; private set; }

    public virtual Company? Company { get; set; }
    
    public int RoleId { get; set; }

    public virtual ApplicationRole Role { get; set; } = null!;

    public virtual ICollection<Defect> Defects { get; set; } = [];
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public void SetCompany(Guid companyId)
    {
        if (CompanyId is not null)
        {
            throw AlreadyWorks.InCompany(CompanyId.Value);
        }
        
        CompanyId = companyId;
    }

    public void RemoveCompany()
    {
        if (CompanyId is not null)
        {
            CompanyId = null;
        }
    }
}