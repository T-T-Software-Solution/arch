using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Loggings;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Securities;
using TTSS.Core.Services;

namespace TTSS.Core;

/// <summary>
/// Helper extension methods for register core modules.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Register core modules.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="assemblies">Related assemblies</param>
    /// <param name="pipelines">Related pipelines</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterTTSSCore(this IServiceCollection target, IEnumerable<Assembly> assemblies, IEnumerable<Type>? pipelines = default)
        => RegisterTTSSCore<CorrelationContext>(target, assemblies, pipelines);

    /// <summary>
    /// Register core modules.
    /// </summary>
    /// <typeparam name="TContext">Correlation context type</typeparam>
    /// <param name="target">The service collection</param>
    /// <param name="assemblies">Related assemblies</param>
    /// <param name="pipelines">Related pipelines</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterTTSSCore<TContext>(this IServiceCollection target, IEnumerable<Assembly> assemblies, IEnumerable<Type>? pipelines = default)
        where TContext : CorrelationContext
        => target
            .RegisterLoggerModule()
            .RegisterCoreServiceModule()
            .RegisterSecurityModule()
            .AddAutoMapper(assemblies)
            .RegisterMessagingModule(assemblies, pipelines?.ToArray() ?? [])
            .AddScoped<ICorrelationContext, TContext>()
            .AddScoped<Lazy<IServiceProvider>>(pvd => new(pvd));
}
