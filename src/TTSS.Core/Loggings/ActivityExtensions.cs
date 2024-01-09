using Microsoft.Extensions.Logging;

namespace TTSS.Core.Loggings;

/// <summary>
/// Helper extension methods for logging.
/// </summary>
public static class ActivityExtensions
{
    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogDebug(exception, "Error while processing request from {Address}", address)</example>
    public static IActivity LogDebug(this IActivity target, Exception? exception, string? message, params object?[] args)
    {
        target.Log(LogLevel.Debug, exception, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogDebug("Processing request from {Address}", address)</example>
    public static IActivity LogDebug(this IActivity target, string? message, params object?[] args)
    {
        target.Log(LogLevel.Debug, null, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogTrace(exception, "Error while processing request from {Address}", address)</example>
    public static IActivity LogTrace(this IActivity target, Exception? exception, string? message, params object?[] args)
    {
        target.Log(LogLevel.Trace, exception, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogTrace("Processing request from {Address}", address)</example>
    public static IActivity LogTrace(this IActivity target, string? message, params object?[] args)
    {
        target.Log(LogLevel.Trace, null, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogInformation(exception, "Error while processing request from {Address}", address)</example>
    public static IActivity LogInformation(this IActivity target, Exception? exception, string? message, params object?[] args)
    {
        target.Log(LogLevel.Information, exception, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogInformation("Processing request from {Address}", address)</example>
    public static IActivity LogInformation(this IActivity target, string? message, params object?[] args)
    {
        target.Log(LogLevel.Information, null, message, args);
        return target;
    }


    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogWarning(exception, "Error while processing request from {Address}", address)</example>
    public static IActivity LogWarning(this IActivity target, Exception? exception, string? message, params object?[] args)
    {
        target.Log(LogLevel.Warning, exception, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogWarning("Processing request from {Address}", address)</example>
    public static IActivity LogWarning(this IActivity target, string? message, params object?[] args)
    {
        target.Log(LogLevel.Warning, null, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogError(exception, "Error while processing request from {Address}", address)</example>
    public static IActivity LogError(this IActivity target, Exception? exception, string? message, params object?[] args)
    {
        target.Log(LogLevel.Error, exception, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogError("Processing request from {Address}", address)</example>
    public static IActivity LogError(this IActivity target, string? message, params object?[] args)
    {
        target.Log(LogLevel.Error, null, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogCritical(exception, "Error while processing request from {Address}", address)</example>
    public static IActivity LogCritical(this IActivity target, Exception? exception, string? message, params object?[] args)
    {
        target.Log(LogLevel.Critical, exception, message, args);
        return target;
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="target">Activity object</param>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <example>target.WriteLogCritical("Processing request from {Address}", address)</example>
    public static IActivity LogCritical(this IActivity target, string? message, params object?[] args)
    {
        target.Log(LogLevel.Critical, null, message, args);
        return target;
    }
}