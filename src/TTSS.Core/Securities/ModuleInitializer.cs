using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Securities;

/// <summary>
/// Helper extension methods for register Security module.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Register JWT describer.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterJwtDescriber(this IServiceCollection services)
    {
        services.AddSingleton<ITokenDescriber, JsonWebTokenDescriber>();
        return services;
    }
}