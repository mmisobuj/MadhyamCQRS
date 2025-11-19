// Example usage file — keep in Examples for reference only.

using Madhyam.Abstractions;
using Madhyam.Logging;

namespace Madhyam.Examples;

public record GetUserQuery(int Id) : IQuery<string>;

[MadhyamLogging(EnumLogLevel.Debug)]
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, string>
{
    public Task<string> HandleAsync(GetUserQuery query, CancellationToken ct = default)
        => Task.FromResult($"User #{query.Id}");
}

public record CreateUserCommand(string Name) : ICommand<int>;

[MadhyamLogging(EnumLogLevel.Information)]
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, int>
{
    public Task<int> HandleAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        // Implement create logic
        return Task.FromResult(Random.Shared.Next(1000, 9999));
    }
}

public record UserCreatedNotification(int Id) : INotification;

public class UserCreatedLogger : INotificationHandler<UserCreatedNotification>
{
    public Task HandleAsync(UserCreatedNotification notification, CancellationToken ct = default)
    {
        Console.WriteLine("User created: " + notification.Id);
        return Task.CompletedTask;
    }
}
