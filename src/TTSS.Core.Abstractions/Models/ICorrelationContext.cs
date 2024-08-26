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
}