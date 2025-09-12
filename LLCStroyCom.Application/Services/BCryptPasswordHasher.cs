using LLCStroyCom.Domain.Services;

namespace LLCStroyCom.Application.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrEmpty(password);
        
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)
                                           || string.IsNullOrEmpty(hashPassword) ||
                                           string.IsNullOrWhiteSpace(hashPassword))
        {
            return false;
        }
        
        return BCrypt.Net.BCrypt.Verify(password, hashPassword);
    }
}