namespace LLCStroyCom.Domain.Services;

public interface ITokenHasher
{
    string HashToken(string token);
}