namespace TTSS.Core.Messaging;

/// <summary>
/// Helper methods for <see cref="IMessagingHub"/>.
/// </summary>
public static class IMessagingHubExtensions
{
    /// <summary>
    /// Sends a remote message to a single remote handler with a response.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="target">Messaging hub</param>
    /// <param name="request">Request object</param>
    /// <param name="timeout">Maximum time to wait for a response</param>
    /// <param name="destinationAddress">Specific service address</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Response from the handler</returns>
    public static Task<TResponse> SendAsync<TRequest, TResponse>(this IMessagingHub target,
        TRequest request,
        TimeSpan timeout = default,
        Uri? destinationAddress = default,
        CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting<TResponse>
        where TResponse : class
        => ((MessagingHub)target).SendAsync<TRequest, TResponse>(request, timeout, destinationAddress, cancellationToken);
}