namespace LLCStroyCom.Infrastructure.Constants;

public static class RoleType
{
    public static readonly string Engineer = "Engineer";
    public static readonly string Manager = "Manager";
    public static readonly string Observer = "Observer";
    public static IReadOnlyCollection<string> AllTypes() => [Engineer, Manager, Observer];
}