using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TTSS.Core.Loggings;

internal sealed class ActivityLogger : IActivity
{
    #region Fields

    private readonly Lazy<ILogger> _logger;
    private readonly Lazy<Activity?> _activity;

    #endregion

    #region Properties

    public Activity? Activity => _activity.Value;
    public string? RootId => _activity.Value?.RootId;
    public string? ParentId => _activity.Value?.ParentId;
    public string? CurrentId => _activity.Value?.Id;
    internal string CallerName { get; }
    internal string LogCategory { get; }

    #endregion

    #region Constructors

    public ActivityLogger(string logCategory,
        string callerName,
        ActivityKind activityKind,
        ILoggerFactory loggerFactory,
        ActivitySource activitySource)
    {
        if (string.IsNullOrWhiteSpace(logCategory))
            throw new ArgumentException($"'{nameof(logCategory)}' cannot be null or whitespace.", nameof(logCategory));

        if (string.IsNullOrWhiteSpace(callerName))
            throw new ArgumentException($"'{nameof(callerName)}' cannot be null or whitespace.", nameof(callerName));

        CallerName = callerName;
        LogCategory = logCategory;
        _logger = new Lazy<ILogger>(() => loggerFactory.CreateLogger(logCategory));
        _activity = new(() => activitySource.StartActivity(callerName, activityKind));
    }

    #endregion

    #region Methods

    public IActivity SetStatus(ActivityStatusCode code, string? description = null)
    {
        _activity.Value?.SetStatus(code, description);
        return this;
    }

    public IActivity AddTag(string key, string? value)
    {
        _activity.Value?.AddTag(key, value);
        return this;
    }

    public IActivity AddTag<T>(string key, T? value)
    {
        _activity.Value?.AddTag(key, value);
        return this;
    }

    public IActivity SetTag(string key, object? value)
    {
        _activity.Value?.SetTag(key, value);
        return this;
    }

    public IActivity AddEvent(ActivityEvent e)
    {
        _activity.Value?.AddEvent(e);
        return this;
    }

    public IActivity AddBaggage(string key, string? value)
    {
        _activity.Value?.AddBaggage(key, value);
        return this;
    }

    public IActivity SetBaggage(string key, string? value)
    {
        _activity.Value?.SetBaggage(key, value);
        return this;
    }

    public IActivity SetUser(string? userId)
    {
        if (false == string.IsNullOrWhiteSpace(userId))
        {
            _activity.Value?.SetBaggage("enduser.id", userId);
        }
        return this;
    }

    public object? GetTag(string key)
        => _activity.Value?.GetTagItem(key);

    public string? GetBaggage(string key)
        => _activity.Value?.GetBaggageItem(key);

    public IActivity Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        var logger = _logger.Value;
        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, exception, message, args);
        }
        return this;
    }

    public IActivity Log(LogLevel logLevel, EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        var logger = _logger.Value;
        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, eventId, exception, message, args);
        }
        return this;
    }

    public void Dispose()
        => _activity.Value?.Dispose();

    #endregion
}