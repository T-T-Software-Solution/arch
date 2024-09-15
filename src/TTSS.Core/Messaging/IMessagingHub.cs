using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

/// <summary>
/// Defines the contract for a messaging hub.
/// </summary>
public interface IMessagingHub
{
    /// <summary>
    /// Publishes a message to all subscribers.
    /// </summary>
    /// <typeparam name="TPublication">Publication type</typeparam>
    /// <param name="request">Notification request</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation</returns>
    Task PublishAsync<TPublication>(TPublication request, CancellationToken cancellationToken = default)
        where TPublication : IPublish;

    /// <summary>
    /// Sends a message to a single handler.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
    Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequest;

    /// <summary>
    /// Sends a message to a single handler with a response.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Response from the handler</returns>
    Task<TResponse> SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a remote message to a single remote handler with a response.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="timeout">Maximum time to wait for a response</param>
    /// <param name="destinationAddress">Specific service address</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Response from the handler</returns>
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request,
        TimeSpan timeout = default,
        Uri? destinationAddress = default,
        CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting<TResponse>
        where TResponse : class;
}