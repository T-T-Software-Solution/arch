using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TTSS.Core.Loggings;

/// <summary>
/// Helper extension methods for create <see cref="IActivity"/>.
/// </summary>
public static class ActivityFactoryExtensions
{
    /// <summary>
    /// Creates an activity with the specified category and caller name.
    /// </summary>
    /// <typeparam name="TCategoryName">Category name</typeparam>
    /// <param name="target">Activity factory object</param>
    /// <param name="callerName">Caller function name</param>
    /// <param name="kind">Activity kind</param>
    /// <exception cref="ArgumentNullException">The IActivityFactory object is required</exception>
    public static IActivity? CreateActivity<TCategoryName>(this IActivityFactory target,
        [CallerMemberName] string callerName = default!,
        ActivityKind kind = ActivityKind.Internal)
        where TCategoryName : class
    {
        if (target is null) throw new ArgumentNullException(nameof(target));
        return target.CreateActivity(typeof(TCategoryName).Name, callerName, kind);
    }

    /// <summary>
    /// Creates an activity with the specified category and caller name.
    /// </summary>
    /// <param name="target">Activity factory object</param>
    /// <param name="caller">Caller object for create category name</param>
    /// <param name="callerName">Caller function name</param>
    /// <param name="kind">Activity kind</param>
    /// <exception cref="ArgumentNullException">The IActivityFactory object is required</exception>
    public static IActivity? CreateActivity(this IActivityFactory target,
        object caller,
        [CallerMemberName] string callerName = default!,
        ActivityKind kind = ActivityKind.Internal)
    {
        if (target is null) throw new ArgumentNullException(nameof(target));
        return target.CreateActivity(caller.GetType().Name, callerName, kind);
    }
}