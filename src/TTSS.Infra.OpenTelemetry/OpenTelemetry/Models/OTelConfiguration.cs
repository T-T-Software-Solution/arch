namespace TTSS.Infra.Loggings.OpenTelemetry.Models;

/// <summary>
/// OpenTelemetry configuration.
/// </summary>
public sealed record OTelConfiguration
{
    #region Properties

    /// <summary>
    /// Service instance Id.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Service namespace.
    /// </summary>
    public required string NameSpace { get; init; }

    /// <summary>
    /// Service name.
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Activity source names.
    /// The list of subscribed sources.
    /// </summary>
    public required IEnumerable<string> ActivitySourceNames { get; init; }

    /// <summary>
    /// The version of the component publishing the tracing info.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// The name of the activity source object.
    /// </summary>
    public required string CurrentSourceName { get; init; }

    #endregion
}