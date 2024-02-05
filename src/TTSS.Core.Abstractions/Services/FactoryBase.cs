using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Services;

/// <summary>
/// Service provider based factory.
/// </summary>
/// <remarks>
/// Initialize an instance of <see cref="FactoryBase"/>.
/// </remarks>
/// <param name="serviceProvider">The service provider</param>
/// <exception cref="ArgumentNullException">The service provider is required</exception>
public abstract class FactoryBase(Lazy<IServiceProvider> serviceProvider)
{
    #region Properties

    private Lazy<IServiceProvider> ServiceProvider => serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    #endregion

    #region Methods

    /// <summary>
    /// Get or create service.
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    /// <returns>The service</returns>
    protected TService? GetOrCreate<TService>()
        => ServiceProvider.Value.GetService<TService>();

    /// <summary>
    /// Get or create service.
    /// </summary>
    /// <param name="serviceType">Service type</param>
    /// <returns>The service</returns>
    protected object? GetOrCreate(Type serviceType)
        => ServiceProvider.Value.GetService(serviceType);

    #endregion
}