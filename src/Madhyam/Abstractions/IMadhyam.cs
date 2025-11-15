namespace Madhyam;

public interface IMadhyam
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification;
}
