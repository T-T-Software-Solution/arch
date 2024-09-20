using System.Text.Json.Serialization;

namespace TTSS.Core.Models;

/// <summary>
/// Contract for correlation context.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Current user ID.
    /// </summary>
    string? CurrentUserId { get; }

    /// <summary>
    /// Correlation ID.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Shared context data repository.
    /// </summary>
    [JsonIgnore]
    IDictionary<string, object> ContextBag { get; }

    #region Methods

    /// <summary>
    /// Tries to get the value from the context bag.
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <param name="value">Data value</param>
    /// <returns>Data value</returns>
    public bool TryGetValue<TData>(out TData? value)
        => TryGetValue(GetKeyName<TData>(), out value);

    /// <summary>
    /// Tries to get the value from the context bag with the specified key.
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <param name="key">Key name</param>
    /// <param name="value">Data value</param>
    /// <returns>Data value</returns>
    public bool TryGetValue<TData>(string key, out TData? value)
    {
        if (false == ContextBag.TryGetValue(key, out var value2))
        {
            value = default;
            return false;
        }

        value = (TData)value2;
        return true;
    }

    /// <summary>
    /// Sets the data in the context bag.
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <param name="value">Data value</param>
    public void Set<TData>(TData value)
        => Set(value, GetKeyName<TData>());

    /// <summary>
    /// Sets the data in the context bag with the specified key.
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <param name="value">Data value</param>
    /// <param name="key">Key name</param>
    public void Set<TData>(TData value, string key)
        => ContextBag[key] = value!;

    /// <summary>
    /// Gets the data from the context bag.
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <returns>Data value</returns>
    public TData Get<TData>()
        => Get<TData>(GetKeyName<TData>());

    /// <summary>
    /// Gets the data from the context bag with the specified key.
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <param name="key">Key name</param>
    /// <returns>Data value</returns>
    public TData Get<TData>(string key)
        => (TData)ContextBag[key];

    private string GetKeyName<TData>()
        => typeof(TData).FullName!;

    #endregion
}