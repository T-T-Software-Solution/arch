using System.Collections.Concurrent;

namespace TTSS.Core.Models;

/// <summary>
/// Default implementation of <see cref="ICorrelationContext"/>.
/// </summary>
public class CorrelationContext : ICorrelationContext, ISetterCorrelationContext
{
    #region Properties

    /// <summary>
    /// Current user ID.
    /// </summary>
    public string? CurrentUserId { get; internal set; }

    /// <summary>
    /// Correlation ID.
    /// </summary>
    public string CorrelationId { get; internal set; } = null!;

    IDictionary<string, object> ICorrelationContext.ContextBag { get; } = new ConcurrentDictionary<string, object>();

    #endregion

    #region Methods

    /// <summary>
    /// Set the current user ID.
    /// </summary>
    /// <param name="currentUserId">The current user ID</param>
    protected void SetCurrentUserIdentity(string? currentUserId)
        => CurrentUserId = currentUserId;

    /// <summary>
    /// Set the correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID</param>
    /// <exception cref="ArgumentNullException">The correlation ID is required</exception>
    protected void SetCorrelationIdentity(string correlationId)
        => CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));

    void ISetterCorrelationContext.SetCorrelationId(string correlationId)
        => SetCorrelationIdentity(correlationId);

    void ISetterCorrelationContext.SetCurrentUserId(string? currentUserId)
        => SetCurrentUserIdentity(currentUserId);

    #endregion
}