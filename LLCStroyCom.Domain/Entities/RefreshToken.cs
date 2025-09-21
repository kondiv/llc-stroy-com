namespace LLCStroyCom.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null && DateTimeOffset.UtcNow < ExpiresAt;
    public virtual ApplicationUser User { get; set; } = null!;
}