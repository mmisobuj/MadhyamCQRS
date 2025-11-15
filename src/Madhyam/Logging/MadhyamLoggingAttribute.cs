namespace Madhyam;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class MadhyamLoggingAttribute : Attribute
{
    public LogLevel Level { get; }

    public MadhyamLoggingAttribute(LogLevel level = LogLevel.Information)
    {
        Level = level;
    }
}

public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}
