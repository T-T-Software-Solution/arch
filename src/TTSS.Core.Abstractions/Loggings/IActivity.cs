namespace TTSS.Core.Loggings;

/// <summary>
/// Activity represents operation with context to be used for logging.
/// </summary>
public interface IActivity : IDisposable
{
    #region Properties

    /// <summary>
    /// The root activity id.
    /// </summary>
    string? RootId { get; }

    /// <summary>
    /// The parent activity id.
    /// </summary>
    string? ParentId { get; }

    /// <summary>
    /// The current activity id.
    /// </summary>
    string? CurrentId { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Update the Activity to have a tag with an additional key and value.
    /// </summary>
    /// <param name="key">The tag key name</param>
    /// <param name="value">The tag value mapped to the input key</param>
    IActivity AddTag(string key, string? value);

    /// <summary>
    /// Update the Activity to have a tag with an additional key and value.
    /// </summary>
    /// <param name="key">The tag key name</param>
    /// <param name="value">The tag value mapped to the input key</param>
    IActivity AddTag<T>(string key, T? value);

    /// <summary>
    /// Add or update the Activity tag with the input key and value.
    /// </summary>
    /// <param name="key">The tag key name</param>
    /// <param name="value">The tag value mapped to the input key</param>
    IActivity SetTag(string key, object? value);

    /// <summary>
    /// Add object to the list.
    /// </summary>
    /// <param name="e"> object of ActivityEvent to add to the attached events list</param>
    IActivity AddEvent(System.Diagnostics.ActivityEvent e);

    /// <summary>
    /// Update the Activity to have baggage with an additional 'key' and value 'value'.
    /// </summary>
    /// <param name="key">The baggage key name</param>
    /// <param name="value">The baggage value mapped to the input key</param>
    IActivity AddBaggage(string key, string? value);

    /// <summary>
    /// Add or update the Activity baggage with the input key and value.
    /// </summary>
    /// <param name="key">The baggage key name</param>
    /// <param name="value">The baggage value mapped to the input key</param>
    IActivity SetBaggage(string key, string? value);

    /// <summary>
    /// Add or update the Activity tag with the user identity.
    /// </summary>
    /// <param name="userId">The user identity</param>
    IActivity SetUser(string? userId);

    /// <summary>
    /// Returns the value of the Activity tag mapped to the input key.
    /// </summary>
    /// <param name="key">The tag key</param>
    /// <returns>The tag value mapped to the input key</returns>
    object? GetTag(string key);

    /// <summary>
    /// Returns the value of the Activity baggage mapped to the input key.
    /// </summary>
    /// <param name="key">The baggage key</param>
    /// <returns>The baggage value mapped to the input key</returns>
    string? GetBaggage(string key);

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logLevel">Entry will be written on this level</param>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">Format string of the log message</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    IActivity Log(Microsoft.Extensions.Logging.LogLevel logLevel, Exception? exception, string? message, params object?[] args);

    #endregion
}