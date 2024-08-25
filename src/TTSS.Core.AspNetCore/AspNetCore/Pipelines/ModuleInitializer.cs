using System.Reflection;
using TTSS.Core.AspNetCore.Models;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace TTSS.Core.AspNetCore.Pipelines;

/// <summary>
/// Helper extension methods for the register TTSS AspNetCore services.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Add the web modules.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="assemblies">Related assemblies</param>
    /// <param name="pipelines">Related pipelines</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterTTSSCoreHttp(this IServiceCollection target, IEnumerable<Assembly> assemblies, IEnumerable<Type>? pipelines = default)
        => RegisterTTSSCoreHttp<WebCorrelationContext>(target, assemblies, pipelines);

    /// <summary>
    /// Add the web modules.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="assemblies">Related assemblies</param>
    /// <param name="pipelines">Related pipelines</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterTTSSCoreHttp<TContext>(this IServiceCollection target, IEnumerable<Assembly> assemblies, IEnumerable<Type>? pipelines = default)
        where TContext : WebCorrelationContext
    {
        Type[] allPipelines =
        [
            typeof(HttpUserIdentityPipelineBehavior<,>),
            typeof(HttpPagingPipelineValidator<,>),
            .. pipelines ?? []
        ];
        return target
            .AddHttpClient()
            .AddHttpContextAccessor()
            .RegisterTTSSCore<TContext>(assemblies, allPipelines)
            .RegisterMessagingModule(assemblies, allPipelines)
            .AddScoped<ICorrelationContext, TContext>();
    }
}