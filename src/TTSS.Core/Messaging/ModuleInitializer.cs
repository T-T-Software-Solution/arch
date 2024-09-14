using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Messaging.Pipelines;

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
        var defaultPipelines = new[]
        {
            typeof(ExceptionPiplineBehaviorAsync<,>),
            typeof(CorrelationPipelineBehaviorAsync<,>),
            typeof(MediatR.Pipeline.RequestPreProcessorBehavior<,>),
            typeof(MediatR.Pipeline.RequestPostProcessorBehavior<,>),
        };
        var target = typeof(IPipelineBehaviorBase<,>);
        var qry = (pipelines ?? [])
            .Where(type => type.IsGenericType
                && type.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == target))
            .Union(defaultPipelines);

        services
            .AddScoped<IMessagingHub, MessagingHub>()
            .AddScoped<ILocalMessagingHub, LocalMessagingHub>()
            .AddMediatR(cfg =>
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