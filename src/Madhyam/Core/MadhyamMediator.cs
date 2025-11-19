using Madhyam.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Madhyam.Core;

public class MadhyamMediator : IMadhyam, ISender, IPublisher
{
    private readonly IServiceProvider _provider;

    public MadhyamMediator(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var type = request.GetType();

        Type handlerType;

        // Route to Query/Command specific handlers first to support specialized pipelines.
        if (typeof(IQuery<TResponse>).IsAssignableFrom(type))
        {
            handlerType = typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TResponse));
        }
        else if (typeof(ICommand<TResponse>).IsAssignableFrom(type))
        {
            handlerType = typeof(ICommandHandler<,>).MakeGenericType(type, typeof(TResponse));
        }
        else
        {
            handlerType = typeof(IRequestHandler<,>).MakeGenericType(type, typeof(TResponse));
        }

        dynamic handler = _provider.GetRequiredService(handlerType);

        return await handler.HandleAsync((dynamic)request, ct);
    }

    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification
    {
        // Resolve all notification handlers registered for this notification type.
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(typeof(TNotification));

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);

        var handlers = (IEnumerable<object>?)_provider.GetService(enumerableType) ?? Array.Empty<object>();

        foreach (dynamic h in handlers)
        {
            await h.HandleAsync((dynamic)notification, ct);
        }
    }
}
