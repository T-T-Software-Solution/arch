namespace TTSS.Core.Models;

/// <summary>
/// Default implementation of <see cref="ICorrelationContext"/>.
/// </summary>
public class CorrelationContext : ICorrelationContext
{
    #region Fields

    private string? _currentUserId;
    private string _correlationId = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Current user ID.
    /// </summary>
    public string? CurrentUserId => _currentUserId;

    /// <summary>
    /// Correlation ID.
    /// </summary>
    public string CorrelationId => _correlationId;

    #endregion

    #region Methods

    /// <summary>
    /// Set the current user ID.
    /// </summary>
    /// <param name="currentUserId">The current user ID</param>
    protected void SetCurrentUserIdentity(string? currentUserId)
        => _currentUserId = currentUserId;

    /// <summary>
    /// Set the correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID</param>
    /// <exception cref="ArgumentNullException">The correlation ID is required</exception>
    protected void SetCorrelationIdentity(string correlationId)
        => _correlationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));

    #endregion
}