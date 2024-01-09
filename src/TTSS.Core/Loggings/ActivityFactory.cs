using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TTSS.Core.Loggings;

internal sealed class ActivityFactory : IActivityFactory
{
    #region Fields

    private readonly Func<string, string, ActivityKind, IActivity?> _create;

    #endregion

    #region Constructors

    public ActivityFactory(ILoggerFactory loggerFactory, ActivitySource activitySource)
    {
        if (loggerFactory is null) throw new ArgumentNullException(nameof(loggerFactory));
        if (activitySource is null) throw new ArgumentNullException(nameof(activitySource));
        _create = (category, callerName, kind) =>
        {
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException($"'{nameof(category)}' cannot be null or WhiteSpace.", nameof(category));
            if (string.IsNullOrWhiteSpace(callerName)) throw new ArgumentException($"'{nameof(callerName)}' cannot be null or WhiteSpace.", nameof(callerName));
            return new ActivityLogger(category, callerName, kind, loggerFactory, activitySource);
        };
    }

    #endregion

    #region Methods

    public IActivity? CreateActivity(string logCategory, string callerName, ActivityKind kind)
        => _create(logCategory, callerName, kind);

    #endregion
}