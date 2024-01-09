using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TTSS.Core.Messaging;

/// <summary>
/// Helper extension methods to register messaging module.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Registers messaging hub and handlers from the specified assemblies.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assemblies">Assemblies to scan</param>
    /// <param name="pipelines">Registers an open behavior type against the <see cref="IPipelineBehaviorBase{TRequest,TResponse}"/> open generic interface type</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection RegisterMessagingModule(this IServiceCollection services, IEnumerable<Assembly> assemblies, params Type[] pipelines)
    {
        var target = typeof(IPipelineBehaviorBase<,>);
        var qry = (pipelines ?? Array.Empty<Type>())
            .Where(type => type.IsGenericType
                && type.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == target))
            .Union(new[]
            {
                typeof(MediatR.Pipeline.RequestPreProcessorBehavior<,>),
                typeof(MediatR.Pipeline.RequestPostProcessorBehavior<,>),
            });

        services.AddSingleton<IMessagingHub>(sp => new MessagingHub(new Lazy<IServiceProvider>(sp)));
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies.ToArray());
            cfg.NotificationPublisherType = typeof(MediatR.NotificationPublishers.TaskWhenAllPublisher);
            foreach (var item in qry)
            {
                cfg.AddOpenBehavior(item, ServiceLifetime.Scoped);
            }
        });
        return services;
    }
}