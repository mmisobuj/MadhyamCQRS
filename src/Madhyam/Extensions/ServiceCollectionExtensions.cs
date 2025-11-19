using Madhyam.Abstractions;
using Madhyam.Core;
using Madhyam.Decorators;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Madhyam.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMadhyam(this IServiceCollection services)
    {
        services.AddSingleton<IMadhyam, MadhyamMediator>();
        return services;
    }

    public static IServiceCollection AddMadhyamWithLogging(this IServiceCollection services)
    {
        services.AddMadhyam();

        // Decorate handlers after registration to enable logging wrappers.
        services.Decorate(typeof(IRequestHandler<,>), typeof(LoggingRequestHandlerDecorator<,>));
        services.Decorate(typeof(INotificationHandler<>), typeof(LoggingNotificationHandlerDecorator<>));

        return services;
    }

    /// <summary>
    /// Registers specific handler implementation types for mediator usage.
    /// </summary>
    public static IServiceCollection AddMadhyamHandlers(this IServiceCollection services, params Type[] types)
    {
        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            foreach (var i in interfaces)
            {
                if (!i.IsGenericType)
                {
                    continue;
                }

                var def = i.GetGenericTypeDefinition();

                if (def == typeof(IRequestHandler<,>) ||
                    def == typeof(IQueryHandler<,>) ||
                    def == typeof(ICommandHandler<,>) ||
                    def == typeof(INotificationHandler<>))
                {
                    services.AddTransient(i, type);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Scans provided assemblies and automatically registers handler types.
    /// </summary>
    public static IServiceCollection AddMadhyamHandlersFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces().Where(i => i.IsGenericType).ToArray();
            foreach (var i in interfaces)
            {
                var def = i.GetGenericTypeDefinition();
                if (def == typeof(IRequestHandler<,>) ||
                    def == typeof(IQueryHandler<,>) ||
                    def == typeof(ICommandHandler<,>) ||
                    def == typeof(INotificationHandler<>))
                {
                    services.AddTransient(i, type);
                }
            }
        }

        return services;
    }
}
