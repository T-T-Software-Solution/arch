using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTSS.Core.Services;
using YamlDotNet.Serialization;

namespace TTSS.Tests;

/// <summary>
/// Base class for all xunit tests with dependency injection.
/// </summary>
public abstract class IoCTestBase : TestBase
{
    #region Properties

    /// <summary>
    /// The service provider.
    /// </summary>
    protected ServiceProvider ServiceProvider { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IoCTestBase"/> class.
    /// </summary>
    public IoCTestBase()
    {
        var services = new ServiceCollection();
        RegisterServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register services into the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    protected abstract void RegisterServices(IServiceCollection services);

    #endregion
}

/// <summary>
/// Base class for all xunit tests with dependency injection and register strategy.
/// </summary>
/// <typeparam name="TInjectionStrategy">Type of DI behavior</typeparam>
public abstract class IoCTestBase<TInjectionStrategy> : IoCTestBase
    where TInjectionStrategy : InjectionStrategyBase
{
    /// <summary>
    /// Register services from <typeparamref name="TInjectionStrategy"/>.
    /// </summary>
    /// <param name="services">The service collection</param>
    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<Fixture>(Fixture);
        services.AddSingleton<ISerializer>(Serializer);
        services.AddSingleton<IDeserializer>(Deserializer);
        services.AddSingleton<IDateTimeService>(DateTimeService);
        services.AddSingleton<ILogger>(LoggerMock.Object);
        services.AddSingleton<ILoggerFactory>(LoggerFactoryMock.Object);
        ActivatorUtilities
            .GetServiceOrCreateInstance<TInjectionStrategy>(services.BuildServiceProvider())
            .RegisterServices(services);
    }
}