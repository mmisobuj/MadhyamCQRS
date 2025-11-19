namespace Madhyam.Abstractions;

public interface IPublisher
{
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification;
}
