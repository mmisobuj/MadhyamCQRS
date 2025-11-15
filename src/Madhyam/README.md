# Madhyam

Madhyam is a lightweight mediator-style library for .NET inspired by MediatR. It provides:

- Request/Response handlers
- Query/Command separation
- Notifications
- Simple logging attribute and decorators
- DI registration helpers and assembly scanning

## Quick start

1. Add package reference or project reference to `Madhyam`.
2. Register in DI:

```csharp
builder.Services.AddLogging();
builder.Services.AddMadhyamWithLogging()
                .AddMadhyamHandlers(typeof(MyQueryHandler), typeof(MyCommandHandler));
```

3. Use IMadhyam:

```csharp
await mediator.SendAsync(new GetUserQuery(id));
await mediator.PublishAsync(new UserCreatedNotification(id));
```
