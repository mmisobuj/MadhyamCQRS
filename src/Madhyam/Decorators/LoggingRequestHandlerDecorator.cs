using Madhyam.Abstractions;
using Madhyam.Logging;
using Microsoft.Extensions.Logging;

namespace Madhyam.Decorators;

public class LoggingRequestHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _inner;
    private readonly ILogger<LoggingRequestHandlerDecorator<TRequest, TResponse>> _logger;
    private readonly EnumLogLevel? _level;

    public LoggingRequestHandlerDecorator(
        IRequestHandler<TRequest, TResponse> inner,
        ILogger<LoggingRequestHandlerDecorator<TRequest, TResponse>> logger)
    {
        _inner = inner;
        _logger = logger;

        var attr = inner.GetType().GetCustomAttributes(typeof(MadhyamLoggingAttribute), false)
                        .FirstOrDefault() as MadhyamLoggingAttribute;

        _level = attr?.Level;
    }

    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default)
    {
        if (_level != null)
        {
            _logger.Log(Map(_level.Value), "Handling {RequestType} - Request: {Request}", typeof(TRequest).Name, System.Text.Json.JsonSerializer.Serialize(request));
        }

        var result = await _inner.HandleAsync(request, ct);

        if (_level != null)
        {
            _logger.Log(Map(_level.Value), "Handled {RequestType} - Response: {Response}", typeof(TRequest).Name, System.Text.Json.JsonSerializer.Serialize(result));
        }

        return result;
    }

    private static Microsoft.Extensions.Logging.LogLevel Map(EnumLogLevel level)
        => level switch
        {
            EnumLogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            EnumLogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            EnumLogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
            EnumLogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            EnumLogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            EnumLogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => Microsoft.Extensions.Logging.LogLevel.Information
        };
}
