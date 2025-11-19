using Madhyam.Abstractions;
using Madhyam.Logging;
using Microsoft.Extensions.Logging;

namespace Madhyam.Decorators;

public class LoggingNotificationHandlerDecorator<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly INotificationHandler<TNotification> _inner;
    private readonly ILogger<LoggingNotificationHandlerDecorator<TNotification>> _logger;
    private readonly EnumLogLevel? _level;

    public LoggingNotificationHandlerDecorator(
        INotificationHandler<TNotification> inner,
        ILogger<LoggingNotificationHandlerDecorator<TNotification>> logger)
    {
        _inner = inner;
        _logger = logger;

        var attr = inner.GetType().GetCustomAttributes(typeof(MadhyamLoggingAttribute), false)
                        .FirstOrDefault() as MadhyamLoggingAttribute;

        _level = attr?.Level;
    }

    public async Task HandleAsync(TNotification notification, CancellationToken ct = default)
    {
        if (_level != null)
        {
            _logger.Log(Map(_level.Value), "Publishing {NotificationType} - Notification: {Notification}", typeof(TNotification).Name, System.Text.Json.JsonSerializer.Serialize(notification));
        }

        await _inner.HandleAsync(notification, ct);

        if (_level != null)
        {
            _logger.Log(Map(_level.Value), "Published {NotificationType}", typeof(TNotification).Name);
        }
    }

    private static LogLevel Map(EnumLogLevel level)
        => level switch
        {
            EnumLogLevel.Trace => LogLevel.Trace,
            EnumLogLevel.Debug => LogLevel.Debug,
            EnumLogLevel.Information => LogLevel.Information,
            EnumLogLevel.Warning => LogLevel.Warning,
            EnumLogLevel.Error => LogLevel.Error,
            EnumLogLevel.Critical => LogLevel.Critical,
            _ => LogLevel.Information
        };
}
