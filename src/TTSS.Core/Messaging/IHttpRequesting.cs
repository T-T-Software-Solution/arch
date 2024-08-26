using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

/// <summary>
/// Contract for a http request message.
/// </summary>
public interface IHttpRequesting : IRequesting<IHttpResponse>;

/// <summary>
/// Contract for a http request message with a response.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IHttpRequesting<TResponse> : IRequesting<IHttpResponse<TResponse>>;