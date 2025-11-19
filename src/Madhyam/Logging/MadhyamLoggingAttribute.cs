namespace Madhyam.Logging;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class MadhyamLoggingAttribute : Attribute
{
    public EnumLogLevel Level { get; }

    public MadhyamLoggingAttribute(EnumLogLevel level = EnumLogLevel.Information)
    {
        Level = level;
    }
}

public enum EnumLogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}
