using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

public interface IMessagingCenter
{
    Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequest;

    Task<TResponse> SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken = default)
       where TResponse : class;

    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request,
        TimeSpan timeout = default,
        Uri? destinationAddress = default,
        CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting<TResponse>
        where TResponse : class;
}

internal sealed class MessagingCenter(Lazy<IServiceProvider> provider) : IMessagingCenter
{
    private IServiceProvider ServiceProvider => provider?.Value ?? throw new InvalidOperationException("The service provider is not available");

    async Task IMessagingCenter.SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is IRemoteRequesting remoteRequest)
        {
            var hub = ServiceProvider.GetRequiredService<IRemoteMessagingHub>();
            await hub.SendAsync(remoteRequest, cancellationToken);
        }
        else if (request is IRequesting localRequest)
        {
            var hub = ServiceProvider.GetRequiredService<IMessagingHub>();
            await hub.SendAsync(localRequest, cancellationToken);
        }
        else
        {
            throw new NotSupportedException($"The request type {request.GetType().Name} is not supported");
        }
    }

    async Task<TResponse> IMessagingCenter.SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var hub = ServiceProvider.GetRequiredService<IMessagingHub>();
        return await hub.SendAsync(request, cancellationToken);
    }

    async Task<TResponse> IMessagingCenter.SendAsync<TRequest, TResponse>(TRequest request, TimeSpan timeout, Uri? destinationAddress, CancellationToken cancellationToken)
    {
        if (request is IRemoteRequesting<TResponse>)
        {
            var hub = ServiceProvider.GetRequiredService<IRemoteMessagingHub>();
            return await hub.SendAsync<TRequest, TResponse>(request, timeout, destinationAddress, cancellationToken);
        }
        else
        {
            throw new NotSupportedException($"The request type {request.GetType().Name} is not supported");
        }
    }
}