using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Messaging.Pipelines;

namespace TTSS.Core;

public static class ModuleInitializer
{
    public static IServiceCollection RegisterRemoteRequest(this IServiceCollection target, string connectionString, IEnumerable<Assembly> assemblies)
    {
        target
            .AddOptions<SqlTransportOptions>()
            .Configure(option => option.ConnectionString = connectionString);

        target
            .AddScoped<IRemoteMessagingHub, RemoteMessagingHub>()
            .AddPostgresMigrationHostedService()
            .AddMassTransit(busCfg =>
            {
                busCfg.AddConsumers([Assembly.GetExecutingAssembly(), .. assemblies]);
                busCfg.UsingPostgres((busContext, factoryCfg) =>
                {
                    factoryCfg.UseConsumeFilter(typeof(CorrelationPipelineFilter<>), busContext);
                    factoryCfg.UseMessageRetry(c => c.Intervals(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));
                    factoryCfg.ConfigureEndpoints(busContext, new RequestingModelName());
                });
            });
        return target;
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