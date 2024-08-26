using System.Text.Json.Serialization;

namespace TTSS.Core.Models;

/// <summary>
/// Contract for correlation context.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Current user ID.
    /// </summary>
    string? CurrentUserId { get; }

    /// <summary>
    /// Correlation ID.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Shared context data repository.
    /// </summary>
    [JsonIgnore]
    IDictionary<string, object> ContextBag { get; }
}