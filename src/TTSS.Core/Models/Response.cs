namespace TTSS.Core.Models;

/// <summary>
/// Represents a response from a request.
/// </summary>
/// <param name="Message">Response message</param>
public record Response(string? Message = default) : IResponse;

/// <summary>
/// Represents a response from a request with data.
/// </summary>
/// <typeparam name="TData">Response data type</typeparam>
/// <param name="Message">Response message</param>
/// <param name="Data">Response data</param>
public record Response<TData>(string? Message = default, TData? Data = default)
    : Response(Message), IResponse<TData>;