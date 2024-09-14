namespace TTSS.Core.Messaging;

/// <summary>
/// Defines a remote messaging hub to encapsulate request/response and publishing interaction patterns.
/// </summary>
public interface IRemoteMessagingHub
{
    /// <summary>
    /// Asynchronously publish a remote notification to multiple handlers.
    /// </summary>
    /// <typeparam name="TPublication">Publication type</typeparam>
    /// <param name="publication">Notification object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation</returns>
    Task PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken = default)
        where TPublication : class;

    /// <summary>
    /// Asynchronously send an object request to a single remote handler.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
    Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting;

    /// <summary>
    /// Asynchronously send a request to a single remote handler with response.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
    /// <param name="destinationAddress">The service address</param>
    /// <param name="cancellationToken">An optional cancellationToken for this request</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request,
        TimeSpan timeout = default,
        Uri? destinationAddress = default,
        CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting<TResponse>
        where TResponse : class;
}