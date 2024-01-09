using System.Net;

namespace TTSS.Core.Models;

/// <summary>
/// Contract for a HTTP response from a request.
/// </summary>
public interface IHttpResponse : IResponse
{
    /// <summary>
    /// Status code.
    /// </summary>
    HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Status code number.
    /// </summary>
    public int StatusCodeNumber => (int)StatusCode;
}

/// <summary>
/// Contract for a HTTP response from a request with data.
/// </summary>
/// <typeparam name="TData">Response data type</typeparam>
public interface IHttpResponse<TData> : IHttpResponse, IResponse<TData>
{
}