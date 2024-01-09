using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Loggings;
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
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterCoreModules(this IServiceCollection target)
    {
        target.RegisterDataTimeService();
        target.RegisterActivityFactory();
        target.RegisterJwtDescriber();
        return target;
    }
}
