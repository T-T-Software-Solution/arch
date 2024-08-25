using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Services;

/// <summary>
/// Helper extension methods for register services module.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Register core services.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterCoreServiceModule(this IServiceCollection target)
        => target
            .AddSingleton<IDateTimeService, DateTimeService>()
            .AddSingleton<IMappingStrategy, AutoMapperMappingStrategy>();
}
