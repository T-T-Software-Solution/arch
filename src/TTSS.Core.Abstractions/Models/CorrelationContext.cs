namespace TTSS.Core.Models;

/// <summary>
/// Default implementation of <see cref="ICorrelationContext"/>.
/// </summary>
public class CorrelationContext : ICorrelationContext
{
    #region Properties

    /// <summary>
    /// Current user ID.
    /// </summary>
    public string? CurrentUserId { get; set; }

    #endregion
}