using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// IServiceCollection extensions.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add interceptors to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="store">The connection store</param>
    /// <param name="builder">The interceptor builder</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInterceptors(this IServiceCollection services, SqlConnectionStore store, SqlInterceptorBuilder builder)
    {
        foreach (var item in builder.InterceptorTypes)
        {
            services.AddKeyedScoped(item, store);
        }
        return services;
    }
}