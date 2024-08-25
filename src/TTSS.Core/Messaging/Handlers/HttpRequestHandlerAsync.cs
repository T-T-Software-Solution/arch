using System.Net;
using System.Reflection;
using TTSS.Core.Annotations;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Defines an asynchronous handler for a http request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public abstract class HttpRequestHandlerAsync<TRequest> : RequestHandlerAsync<TRequest, IHttpResponse>
    where TRequest : IRequesting<IHttpResponse>
{
    #region Methods

    /// <summary>
    /// Creates a response with the specified status code and message.
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Message</param>
    /// <returns>The response</returns>
    protected IHttpResponse Response(HttpStatusCode statusCode, string? message = default)
        => new HttpResponse(statusCode, message)
        {
            Metadata = typeof(TRequest).GetCustomAttribute<OperationDescriptionAttribute>()
        };

    #endregion
}

/// <summary>
/// Defines an asynchronous handler for a http request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public abstract class HttpRequestHandlerAsync<TRequest, TResponse> : RequestHandlerAsync<TRequest, IHttpResponse<TResponse>>
    where TRequest : IRequesting<IHttpResponse<TResponse>>
{
    #region Methods

    /// <summary>
    /// Creates a response with the specified status code and message.
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Message</param>
    /// <returns>The response</returns>
    protected IHttpResponse<TResponse> Response(HttpStatusCode statusCode, string? message = default)
        => new HttpResponse<TResponse>(statusCode, message)
        {
            Metadata = typeof(TRequest).GetCustomAttribute<OperationDescriptionAttribute>()
        };

    /// <summary>
    /// Creates a response with the specified status code and message.
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="result">Response data</param>
    /// <param name="message">Message</param>
    /// <returns>The response</returns>
    protected IHttpResponse<TResponse> Response(HttpStatusCode statusCode, TResponse result, string? message = default)
        => new HttpResponse<TResponse>(statusCode, message, result)
        {
            Metadata = typeof(TRequest).GetCustomAttribute<OperationDescriptionAttribute>()
        };

    #endregion
}