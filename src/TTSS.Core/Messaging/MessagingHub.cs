using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Messaging;

internal sealed class MessagingHub(Lazy<IServiceProvider> provider) : IMessagingHub
{
    #region Properties

    private ILocalMessagingHub LocalHub => ServiceProvider.GetRequiredService<ILocalMessagingHub>();
    private IRemoteMessagingHub RemoteHub => ServiceProvider.GetRequiredService<IRemoteMessagingHub>();
    private IServiceProvider ServiceProvider => provider?.Value ?? throw new InvalidOperationException("The service provider is not available");

    #endregion

    #region Methods

    Task IMessagingHub.PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken)
    {
        // TODO: Implement the method for remote publication
        ArgumentNullException.ThrowIfNull(publication);
        return LocalHub.PublishAsync(publication, cancellationToken);
    }

    Task IMessagingHub.SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is IRemoteRequesting remoteRequest)
        {
            return RemoteHub.SendAsync(remoteRequest, cancellationToken);
        }
        else if (request is IRequesting localRequest)
        {
            return LocalHub.SendAsync(localRequest, cancellationToken);
        }
        else
        {
            throw new NotSupportedException($"The request type {request.GetType().Name} is not supported");
        }
    }

    Task<TResponse> IMessagingHub.SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return LocalHub.SendAsync(request, cancellationToken);
    }

    Task<TResponse> IMessagingHub.SendAsync<TRequest, TResponse>(TRequest request, TimeSpan timeout, Uri? destinationAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not IRemoteRequesting<TResponse>)
        {
            throw new NotSupportedException($"The request type {request.GetType().Name} is not supported");
        }

        return RemoteHub.SendAsync<TRequest, TResponse>(request, timeout, destinationAddress, cancellationToken);
    }
}

#endregion