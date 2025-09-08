namespace LLCStroyCom.Domain.Constants;

public static class RoleType
{
    public const string Engineer = "engineer";
    public const string Manager = "manager";
    public const string Observer = "observer";
    public static IReadOnlyCollection<string> AllTypes() => [Engineer, Manager, Observer];
}