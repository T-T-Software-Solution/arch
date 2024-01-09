using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;
using YamlDotNet.Serialization;

namespace TTSS.Tests;

/// <summary>
/// Dependency injection strategy base class.
/// </summary>
public abstract class InjectionStrategyBase
{
    #region Properties

    /// <summary>
    /// Object creation services.
    /// </summary>
    protected Fixture Fixture { get; }

    /// <summary>
    /// Yaml serializer.
    /// </summary>
    protected ISerializer Serializer { get; }

    /// <summary>
    /// Yaml deserializer.
    /// </summary>
    protected IDeserializer Deserializer { get; }

    /// <summary>
    /// Date time service.
    /// </summary>
    protected IDateTimeService DateTimeService { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectionStrategyBase"/> class.
    /// </summary>
    /// <param name="fixture">Object creation services.</param>
    /// <param name="serializer">Object serializer</param>
    /// <param name="deserializer">Object deserializer</param>
    /// <param name="dateTimeService">Date time service</param>
    protected InjectionStrategyBase(Fixture fixture, ISerializer serializer, IDeserializer deserializer, IDateTimeService dateTimeService)
    {
        Fixture = fixture;
        Serializer = serializer;
        Deserializer = deserializer;
        DateTimeService = dateTimeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register services into the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    public abstract void RegisterServices(IServiceCollection services);

    #endregion
}