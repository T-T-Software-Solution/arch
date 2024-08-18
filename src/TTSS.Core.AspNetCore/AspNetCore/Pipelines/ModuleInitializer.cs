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
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterWebModules(this IServiceCollection target)
        => target
            .AddHttpClient()
            .AddHttpContextAccessor();
}