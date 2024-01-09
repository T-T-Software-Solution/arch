using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using TTSS.Core.Loggings;
using TTSS.Infra.Loggings.OpenTelemetry.Models;

namespace TTSS.Infra.Loggings.OpenTelemetry;

/// <summary>
/// Helper extension methods for register OpenTelemetry.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Register OpenTelemetry module.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="oTelConfiguration">The OpenTelemetry configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterOpenTelemetry(this IServiceCollection services, OTelConfiguration oTelConfiguration)
        => AddOpenTelemetry(services, oTelConfiguration, false);

    /// <summary>
    /// Register OpenTelemetry module with Azure Monitor.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="oTelConfiguration">The OpenTelemetry configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterOpenTelemetryOnAzureMonitor(this IServiceCollection services, OTelConfiguration oTelConfiguration)
        => AddOpenTelemetry(services, oTelConfiguration, true);

    private static IServiceCollection AddOpenTelemetry(IServiceCollection services, OTelConfiguration oTelConfiguration, bool useAzureMonitor)
    {
        var isConfigurationValid = oTelConfiguration is not null
           && !string.IsNullOrWhiteSpace(oTelConfiguration.Id)
           && !string.IsNullOrWhiteSpace(oTelConfiguration.ServiceName)
           && !string.IsNullOrWhiteSpace(oTelConfiguration.NameSpace)
           && !string.IsNullOrWhiteSpace(oTelConfiguration.Version)
           && !string.IsNullOrWhiteSpace(oTelConfiguration.CurrentSourceName)
           && (oTelConfiguration.ActivitySourceNames?.Any() ?? false);
        if (!isConfigurationValid) throw new ArgumentException("OpenTelemetry configuration is not valid.");

        services.ConfigureOpenTelemetryTracerProvider((sp, builder) =>
        {
            builder.AddSource(oTelConfiguration!.ActivitySourceNames!.ToArray());
            builder.ConfigureResource(cfg => cfg.AddAttributes(new Dictionary<string, object>
            {
                { "service.instance.id", oTelConfiguration.Id },
                { "service.name", oTelConfiguration.ServiceName },
                { "service.namespace", oTelConfiguration.NameSpace },
            }));
        });
        var oTelBuilder = services.AddOpenTelemetry();
        if (useAzureMonitor)
        {
            oTelBuilder.UseAzureMonitor();
        }
        services.RegisterActivityFactory();
        services.AddSingleton<ActivitySource>(pvd => new(oTelConfiguration!.CurrentSourceName, oTelConfiguration.Version));
        return services;
    }
}