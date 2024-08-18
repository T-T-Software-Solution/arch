namespace TTSS.Core.Models;

/// <summary>
/// Contract for setting the correlation context.
/// </summary>
public interface ISetterCorrelationContext
{
    /// <summary>
    /// Set the current user ID.
    /// </summary>
    /// <param name="currentUserId">The current user ID</param>
    void SetCurrentUserId(string currentUserId);

    /// <summary>
    /// Set the correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID</param>
    void SetCorrelationId(string correlationId);
}