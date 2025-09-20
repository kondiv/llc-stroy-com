namespace LLCStroyCom.Domain.Specifications;

public abstract class OrderBy
{
    public abstract string Type { get; }
    public bool Descending { get; set; } = false;
}