namespace TTSS.Core.Data;

/// <summary>
/// Contract for database warmup.
/// </summary>
public interface IDbWarmup
{
    /// <summary>
    /// Warms up the database.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task WarmupAsync();
}