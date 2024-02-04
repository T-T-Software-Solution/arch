namespace TTSS.Core.Models;

/// <summary>
/// Helper extensions for <see cref="System.Net.HttpStatusCode"/>.
/// </summary>
public static class HttpStatusCodeExtensions
{
    /// <summary>
    /// Creates a <see cref="IResponse"/> from a <see cref="System.Net.HttpStatusCode"/>.
    /// </summary>
    /// <param name="statusCode">Status code</param>
    /// <param name="message">Response message</param>
    /// <returns>Returns a <see cref="IResponse"/>.</returns>
    public static HttpResponse ToResponse(this System.Net.HttpStatusCode statusCode, string? message = default)
        => new(statusCode, message);

    /// <summary>
    /// Creates a <see cref="IResponse{TData}"/> from a <see cref="System.Net.HttpStatusCode"/>.
    /// </summary>
    /// <typeparam name="TData">Response data type</typeparam>
    /// <param name="statusCode">Status code</param>
    /// <param name="message">Response message</param>
    /// <param name="data">Data to include in the response</param>
    /// <returns>Returns a <see cref="IResponse{TData}"/>.</returns>
    public static HttpResponse<TData> ToResponse<TData>(this System.Net.HttpStatusCode statusCode, string? message = default, TData? data = default)
        => new(statusCode, message, data);
}