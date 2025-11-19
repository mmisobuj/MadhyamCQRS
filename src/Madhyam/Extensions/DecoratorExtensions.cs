using Microsoft.Extensions.DependencyInjection;

namespace Madhyam.Extensions;

public static class DecoratorExtensions
{
    // Simple decorator helper; assumes handlers are registered as concrete types first.
    public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Type decoratorType)
    {
        var descriptors = services.Where(s => s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == serviceType).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);

            services.Add(new ServiceDescriptor(
                descriptor.ServiceType,
                provider =>
                {
                    var original = provider.GetRequiredService(descriptor.ImplementationType ?? descriptor.ServiceType);
                    return ActivatorUtilities.CreateInstance(provider, decoratorType, original);
                },
                descriptor.Lifetime));
        }

        return services;
    }
}
