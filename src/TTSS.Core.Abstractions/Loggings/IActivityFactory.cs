namespace TTSS.Core.Loggings;

/// <summary>
/// Contract for creating activities.
/// </summary>
public interface IActivityFactory
{
    /// <summary>
    /// Create an activity.
    /// </summary>
    /// <param name="logCategory">Log category name</param>
    /// <param name="callerName">Caller function name</param>
    /// <param name="kind">Activity kind</param>
    /// <returns>Activity</returns>
    IActivity? CreateActivity(string logCategory, string callerName, System.Diagnostics.ActivityKind kind);
}