namespace TTSS.Core.Models;

/// <summary>
/// Represents a HTTP response from a request.
/// </summary>
/// <param name="StatusCode">Status code</param>
/// <param name="Message">Response message</param>
public record HttpResponse(System.Net.HttpStatusCode StatusCode, string? Message = default)
    : Response(Message), IHttpResponse;

/// <summary>
/// Represents a HTTP response from a request with data.
/// </summary>
/// <typeparam name="TData">Response data type</typeparam>
/// <param name="StatusCode">Status code</param>
/// <param name="Message">Response message</param>
/// <param name="Data">Response data</param>
public record HttpResponse<TData>(System.Net.HttpStatusCode StatusCode, string? Message = default, TData? Data = default)
    : Response<TData>(Message, Data), IHttpResponse<TData>;