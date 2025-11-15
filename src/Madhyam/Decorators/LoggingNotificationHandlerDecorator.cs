using Microsoft.Extensions.Logging;

namespace Madhyam;

public class LoggingNotificationHandlerDecorator<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly INotificationHandler<TNotification> _inner;
    private readonly ILogger<LoggingNotificationHandlerDecorator<TNotification>> _logger;
    private readonly LogLevel? _level;

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

    private static Microsoft.Extensions.Logging.LogLevel Map(LogLevel level)
        => level switch
        {
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => Microsoft.Extensions.Logging.LogLevel.Information
        };
}
