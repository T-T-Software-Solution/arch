namespace TTSS.Core.Models;

/// <summary>
/// Contract for a response from a request.
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Response message.
    /// </summary>
    string? Message { get; }
}

/// <summary>
/// Contract for a response from a request with data.
/// </summary>
/// <typeparam name="TData">Response data type</typeparam>
public interface IResponse<TData> : IResponse
{
    /// <summary>
    /// Response data.
    /// </summary>
    TData? Data { get; }
}