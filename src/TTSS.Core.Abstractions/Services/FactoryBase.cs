using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Services;

/// <summary>
/// Service provider based factory.
/// </summary>
public abstract class FactoryBase
{
    #region Fields

    private readonly Lazy<IServiceProvider> _serviceProvider;

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="FactoryBase"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <exception cref="ArgumentNullException">The service provider is required</exception>
    public FactoryBase(Lazy<IServiceProvider> serviceProvider)
        => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    #endregion

    #region Methods

    /// <summary>
    /// Get or create service.
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    /// <returns>The service</returns>
    protected TService? GetOrCreate<TService>()
        => _serviceProvider.Value.GetService<TService>();

    /// <summary>
    /// Get or create service.
    /// </summary>
    /// <param name="serviceType">Service type</param>
    /// <returns>The service</returns>
    protected object? GetOrCreate(Type serviceType)
        => _serviceProvider.Value.GetService(serviceType);

    #endregion
}