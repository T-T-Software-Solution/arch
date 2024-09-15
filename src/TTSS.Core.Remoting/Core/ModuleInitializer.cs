using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Messaging.Pipelines;

namespace TTSS.Core;

/// <summary>
/// Helper extension methods for register remote messaging module.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Register remote messaging module.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="assemblies">Related assemblies</param>
    /// <param name="busConfigure">Bus configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterRemoteRequest(this IServiceCollection target, IEnumerable<Assembly> assemblies, Action<IBusRegistrationConfigurator, Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>> busConfigure)
    {
        target
            .AddScoped<IRemoteMessagingHub, RemoteMessagingHub>()
            .AddMassTransit(busCfg =>
            {
                busCfg.AddConsumers([Assembly.GetExecutingAssembly(), .. assemblies]);
                busConfigure(busCfg, ConfigureBus);
            });
        return target;
    }

    private static void ConfigureBus(IBusRegistrationContext busContext, ISqlBusFactoryConfigurator factoryCfg)
    {
        var consumeFilters = new[]
        {
            typeof(CorrelationPipelineFilter<>),
            typeof(UserIdentityPipelineFilter<>),
        };
        foreach (var item in consumeFilters)
        {
            factoryCfg.UseConsumeFilter(item, busContext);
        }
        factoryCfg.UseMessageRetry(cfg => cfg.Intervals(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));
        factoryCfg.ConfigureEndpoints(busContext, new RequestingModelName());
    }
}

file sealed class RequestingModelName : DefaultEndpointNameFormatter
{
    public override string Consumer<TRequest>()
    {
        var requestingType = typeof(TRequest).BaseType;
        if (requestingType?.IsAssignableTo(typeof(RemoteRequestHandlerAsync)) ?? false)
        {
            var requestingName = requestingType.GetGenericArguments().First().Name;
            return requestingName;
        }
        return base.Consumer<TRequest>();
    }
}