using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Loggings;

/// <summary>
/// Helper extension methods for register Logging module.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Register ActivityFactory.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterActivityFactory(this IServiceCollection target)
        => target.AddTransient<IActivityFactory, ActivityFactory>();
}